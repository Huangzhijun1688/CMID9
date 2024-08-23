using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO; // 引用System.IO命名空间，用于Path.Combine方法

namespace CMID9_DEV
{


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new 系统登录());
        }
    }


    public static class _Sys
    {
        public static string PassKey = "HNyt2019"; // 整个项目加密字符串时使用的密匙
        public static string Path = AppDomain.CurrentDomain.BaseDirectory; // 程序启动路径
        public static string LinkFile = System.IO.Path.Combine(Path, "Link_Info.Txt"); // 使用Path.Combine方法确保路径正确并拼接文件名
        public static int LoadRecID = 0; // 登录日志的记录ID [M,Task_Log,_Identify]
        public static DateTime InDate = DateTime.Now; // 启动系统的日期时间
        public static DateTime LoadDate = DateTime.Now; // 登录系统的日期时间
        public static DateTime ExitDate = DateTime.Now; // 退出系统的日期时间
        public static string PCID = ""; // 终端ID [M,PC_Info,终端ID]
        public static string PCName = ""; // 终端名称 [M,PC_Info,终端名称]
        public static string PCIsRegi = ""; // 终端是否注册 [M,PC_Info,是否注册]
        public static string PCIsDisa = ""; // 终端是否禁用 [M,PC_Info,禁用]
        public static bool Passinitial = false; // 退出系统时是否提示
    }


    public static class _Boot
    {
        public static string BasePath = Path.Combine(_Sys.Path); // 程序文件启动的路径
        public static string SysPath = Path.Combine(_Sys.Path, "Sys"); // 使用Path.Combine确保系统文件路径正确
        public static string UserPath = Path.Combine(_Sys.Path, "User" + _LoadUser.Vip_Id); // 使用Path.Combine确保系统文件路径正确
        public static string LoadFile = Path.Combine(_Sys.Path, _LoadUser.Vip_Id + "_Load.Txt"); // 加载用户信息的文件路径正确
    }


public static class _LoadUser
    {
        public static bool Is_Load = false; // 是否允许用户登录
        public static string Vip_Name = ""; // 登录用户的姓名 [M,VIP_Arct,Vip_Name]
        public static string Vip_Id = ""; // 登录用户的ID [M,VIP_Arct,Vip_ID]
        public static string Vip_Acc = ""; // 登录用户的显示名称 [M,VIP_Arct,Vip_Acc]
        public static string Vip_Sex = ""; // 登录用户的性别 [M,VIP_Arct,Vip_Sex]
        public static string Vip_Name_En = ""; // 登录用户使用的英文名称 [M,VIP_Arct,Vip_Name_En]
        public static string Vip_Name_Cn = ""; // 登录用户使用的中文名称 [M,VIP_Arct,Vip_Name_Cn]
        public static string PassWordPC9 = ""; // 登录用户加密后的密码字符串 [M,VIP_Arct,Vip_PassWordPC9]
        public static string WXID = ""; // 登录用户的微信ID [M,VIP_Arct,WXID]
        public static string Abbr_List = ""; // 登录用户允许访问的企业列表 [M,VIP_Arct,DataSysTemp]
        public static string Abbr = ""; // 登录用户选择的企业
        public static string Vip_Grou = ""; // 登录用户所在工作组，组名称使用{},组与组之间使用 | 分隔
        public static string Passinitial = Vip_Id + "123"; // 登录用户的加密后的初始密码
        public static string Confile = _Boot.UserPath + "Loader.Txt" ; // 登录用户的配置文件

    }



    public static class _Company
    {
        public static string ID = ""; // 登录用户选择的企业ID
        public static string Abbr = ""; // 登录用户选择的企业简称
        public static string Name = ""; // 登录用户选择的企业名称
        public static string Name_En = ""; // 登录用户选择的企业英文名称
        public static string Abbr_List = ""; // 登录用户允许访问的企业列表，用 | 分隔
        public static string Appr_List = ""; // 登录用户选择企业的审批人列表，用 | 分隔
    }



    public static class _Connection
    {
        public static string Main_File = Path.Combine(_Boot.SysPath, "Link_Info.txt"); // 系统连接文件
        public static string Main_Str = ""; // 系统连接字符串
        public static string Conf_Str = ""; // 配置连接字符串
        public static string Data_Str = ""; // 数据连接字符串

        public static string Main_Server = ""; // 系统连接主机的IP或主机名
        public static string Main_Database = ""; // 系统连接的数据库名称
        public static string Main_UserID = ""; // 系统连接数据库用户的用户名
        public static string Main_Password = ""; // 系统连接数据库用户的密码
        public static string Main_Port = ""; // 系统连接服务器的端口号
        public static string Main_Type = ""; // 系统连接数据库的类型,例:MY SQL

        public static string Conf_Server = ""; // 配置连接主机的IP或主机名
        public static string Conf_Database = ""; // 配置连接的数据库名称
        public static string Conf_UserID = ""; // 配置连接数据库用户的用户名
        public static string Conf_Password = ""; // 配置连接数据库用户的密码
        public static string Conf_Port = ""; // 配置连接服务器的端口号
        public static string Conf_Type = ""; // 系统连接数据库的类型,例:MY SQL


        public static string Data_Server = ""; // 数据连接主机的IP或主机名
        public static string Data_Database = ""; // 数据连接的数据库名称
        public static string Data_UserID = ""; // 数据连接数据库用户的用户名
        public static string Data_Password = ""; // 数据连接数据库用户的密码
        public static string Data_Port = ""; // 数据连接服务器的端口号
        public static string Data_Type = ""; // 系统连接数据库的类型,例:MY SQL


    }


}
