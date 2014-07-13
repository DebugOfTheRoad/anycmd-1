
namespace Anycmd.Util
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text.RegularExpressions;

    /// <summary>
    /// IP访问助手
    /// </summary>
    public static class IPHelper
    {
        #region GetLocalIP
        /// <summary>
        /// 得到本机IP
        /// </summary>
        public static string GetLocalIP()
        {
            string strLocalIP = "";
            string strPcName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strPcName);
            foreach (var ip in ipEntry.AddressList)
            {
                if (IsRightIP(ip.ToString()))
                {
                    strLocalIP = ip.ToString();
                    break;
                }
            }

            return strLocalIP;
        }
        #endregion

        public static HashSet<string> GetLocalIPs()
        {
            var ips = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            string strPcName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strPcName);
            foreach (var ip in ipEntry.AddressList)
            {
                if (IsRightIP(ip.ToString()))
                {
                    ips.Add(ip.ToString());
                }
            }

            return ips;
        }

        #region GetGateway
        /// <summary>
        /// 得到网关地址
        /// </summary>
        /// <returns></returns>
        public static string GetGateway()
        {
            string strGateway = "";
            //获取所有网卡
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netWork in nics)
            {
                IPInterfaceProperties ip = netWork.GetIPProperties();
                GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;
                foreach (var gateWay in gateways)
                {
                    if (IsPingIP(gateWay.Address.ToString()))
                    {
                        strGateway = gateWay.Address.ToString();
                        break;
                    }
                }

                if (strGateway.Length > 0)
                {
                    break;
                }
            }

            return strGateway;
        }
        #endregion

        #region IsRightIP
        /// <summary>
        /// 判断是否为正确的IP地址
        /// </summary>
        /// <param name="strIPadd">需要判断的字符串</param>
        /// <returns>true = 是 false = 否</returns>
        public static bool IsRightIP(string strIPadd)
        {
            if (Regex.IsMatch(strIPadd, "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}"))
            {
                //根据小数点分拆字符串
                string[] ips = strIPadd.Split('.');
                if (ips.Length == 4 || ips.Length == 6)
                {
                    if (System.Int32.Parse(ips[0]) < 256 && System.Int32.Parse(ips[1]) < 256 & System.Int32.Parse(ips[2]) < 256 & System.Int32.Parse(ips[3]) < 256)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion

        #region IsPingIP
        /// <summary>
        /// 尝试Ping指定IP是否能够Ping通
        /// </summary>
        /// <param name="strIP">指定IP</param>
        /// <returns>true 是 false 否</returns>
        public static bool IsPingIP(string strIP)
        {
            try
            {
                var ping = new Ping();
                PingReply reply = ping.Send(strIP, 1000);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
