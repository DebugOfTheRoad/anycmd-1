using ServiceStack;

namespace Anycmd.EDI.MessageServices
{
    using Host.EDI;
    using Host.EDI.Hecp;
    using ServiceModel.Operations;
    using ServiceModel.Types;
    using System.Linq;
    using Util;

    /// <summary>
    /// 数据交换上下文查询服务。
    /// </summary>
    public sealed class EDIContextService : Service
    {
        public EDIContextService()
        {
        }

        private IAppHost host
        {
            get
            {
                return System.Web.HttpContext.Current.Application["AppHostInstance"] as IAppHost;
            }
        }

        #region GetAllStateCodes
        public GetAllStateCodesResponse Any(GetAllStateCodes request)
        {
            var response = new GetAllStateCodesResponse()
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName()
            };
            ApiVersion apiVersion;
            if (request == null || !request.Version.TryParse(out apiVersion))
            {
                response.Description = "非法的版本号" + request.Version;
                return response;
            }
            foreach (var stateCode in host.StateCodes)
            {
                response.StateCodes.Add(new StateCodeData
                {
                    Description = stateCode.Description,
                    ReasonPhrase = stateCode.ReasonPhrase,
                    StateCode = stateCode.Code
                });
            }

            return response;
        }
        #endregion

        #region GetAllOntologies
        public GetAllOntologiesResponse Any(GetAllOntologies request)
        {
            var ontologies = host.Ontologies;
            var response = new GetAllOntologiesResponse()
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName()
            };

            foreach (var ontology in ontologies.OrderBy(a => a.Ontology.SortCode))
            {
                if (ontology.Ontology.IsEnabled == 1)
                {
                    response.Ontologies.Add(ontology.ToOntologyData());
                }
            }

            return response;
        }
        #endregion

        #region GetOntology
        public GetOntologyResponse Any(GetOntology request)
        {
            OntologyDescriptor ontology;
            if (request == null || !host.Ontologies.TryGetOntology(request.OntologyCode, out ontology) || ontology.Ontology.IsEnabled != 1)
            {
                if (string.IsNullOrEmpty(request.OntologyCode))
                {
                    return new GetOntologyResponse()
                    {
                        Status = (int)Status.Ok,
                        ReasonPhrase = Status.Ok.ToName(),
                        Description = "传入的参数错误，OntologyCode不能为空"
                    };
                }
                return new GetOntologyResponse()
                {
                    Status = (int)Status.NotExist,
                    ReasonPhrase = Status.NotExist.ToName(),
                    Description = "编码为" + request.OntologyCode + "的本体不存在"
                };
            }

            return new GetOntologyResponse
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName(),
                Ontology = ontology.ToOntologyData()
            };
        }
        #endregion

        #region GetAllInfoDics
        public GetInfoDicsResponse Any(GetAllInfoDics request)
        {
            var infoDics = host.InfoDics;
            var infoDicsData = new GetInfoDicsResponse()
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName()
            };
            foreach (var infoDic in infoDics.OrderBy(a => a.SortCode))
            {
                infoDicsData.InfoDics.Add(infoDic.ToInfoDicData());
            }

            return infoDicsData;
        }
        #endregion

        #region GetInfoDic
        public GetInfoDicResponse Any(GetInfoDic request)
        {
            InfoDicState infoDic;
            if (request == null || !host.InfoDics.TryGetInfoDic(request.DicCode, out infoDic) || infoDic.IsEnabled != 1)
            {
                if (string.IsNullOrEmpty(request.DicCode))
                {
                    return new GetInfoDicResponse()
                    {
                        Status = (int)Status.InvalidArgument,
                        ReasonPhrase = Status.InvalidArgument.ToName(),
                        Description = "传入的参数错误，DicCode不能为空"
                    };
                }
                return new GetInfoDicResponse()
                {
                    Status = (int)Status.NotExist,
                    ReasonPhrase = Status.NotExist.ToName(),
                    Description = "编码为" + request.DicCode + "的字典不存在"
                };
            }

            return new GetInfoDicResponse
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName(),
                InfoDic = infoDic.ToInfoDicData()
            };
        }
        #endregion

        #region GetAllOrganizations
        public GetOrganizationsResponse Any(GetAllOrganizations request)
        {
            var organizations = new GetOrganizationsResponse()
            {
                Status = (int)Status.Ok,
                ReasonPhrase = Status.Ok.ToName()
            };
            if (host.OrganizationSet != null)
            {
                foreach (var item in host.OrganizationSet)
                {
                    var serializableOrganization = new OrganizationData()
                    {
                        ParentCode = item.Parent.Code,
                        Name = item.Name,
                        Code = item.Code
                    };
                    organizations.Organizations.Add(serializableOrganization);
                }
            }

            return organizations;
        }
        #endregion
    }
}
