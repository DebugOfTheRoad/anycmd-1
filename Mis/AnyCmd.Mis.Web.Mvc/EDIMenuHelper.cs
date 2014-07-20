
namespace Anycmd.Mis.Web.Mvc
{
    using AC.Infra;
    using Host;
    using Host.AC.Infra;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class EDIMenuHelper
    {
        private static readonly Menu entityMenu = new Menu
        {
            Id = new Guid("9BE27152-808E-427D-B8C6-D7ABC600A963"),
            ParentID = null,
            Name = "实体管理",
            Url = string.Empty,
            Icon = string.Empty
        };

        public static IList<IMenu> GetEntityMenus()
        {
            var host = HttpContext.Current.Application["AppHostInstance"] as IAppHost;
            IList<IMenu> menus = new List<IMenu>();
            menus.Add(entityMenu);
            foreach (var ontology in host.Ontologies.OrderBy(o => o.Ontology.SortCode))
            {
                if (ontology.Ontology.IsEnabled == 1)
                {
                    if (!ontology.Ontology.IsSystem
                        || (ontology.Ontology.IsSystem && host.UserSession.IsDeveloper()))
                    {
                        menus.Add(new Menu
                        {
                            Id = ontology.Ontology.Id,
                            ParentID = entityMenu.Id,
                            Name = ontology.Ontology.Name + "管理",
                            Url = string.Format("EDI/Entity/Index?ontologyCode={0}&ontologyID={1}", ontology.Ontology.Code, ontology.Ontology.Id),
                            Icon = ontology.Ontology.Icon
                        });
                    }
                }
            }

            return menus;
        }
    }
}
