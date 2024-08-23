using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CMID9_DEV.Models; // 引用包含ConnectionHelper类的命名空间


// 我现在改这里,试一下,感觉不对,这是在仓库里启动的项目文件,修改的

namespace CMID9_DEV
{
    public partial class 系统登录 : DevExpress.XtraEditors.XtraForm
    {
        public 系统登录()
        {
            InitializeComponent();
            Txt密码.Properties.PasswordChar = '*'; // 设置 Txt密码 为密文 *，

        }


        private void 系统登录_Load(object sender, EventArgs e)
        {
            bool connectionInitialized = ConnectionHelper.InitializeMainConnection(); // 调用检查并初始化系统连接方法

            if (connectionInitialized) // 如果返回TRUE
            {
                But登录.Enabled = true; // 启用登录按钮
            }
            else // 如果返回FALSE
            {
                But登录.Enabled = false; // 禁用登录按钮
            }
        }

            private void txt帐号_Validated(object sender, EventArgs e)
            {
                ProcessInput(); // 调用处理输入的逻辑
            }


        #region 获取有效企业列表
        private List<string> GetValidCompanies(string[] companyList) // 获取有效企业列表方法开始
        {
            List<string> validCompanies = new List<string>(); // 创建一个存储有效企业的列表

            foreach (string companyAbbr in companyList) // 遍历传入的企业列表
            {
                string condition = $"CompanyAbbr = '{companyAbbr}'"; // 构建查询条件

                object result = DatabaseHelper.GetColumnValue("M", "App_Register", "CompanyAbbr", condition); // 查询 App_Register 表，检查是否存在该企业缩写

                if (result != null) // 如果查询结果不为空，说明匹配记录存在
                {
                    validCompanies.Add(companyAbbr); // 将有效企业添加到列表中
                }
            }

            return validCompanies; // 返回有效企业列表
        }
        #endregion

        #region 输入获取:姓名、企业等
        private void ProcessInput()
        {
            #region 获取用户输入并检查是否为空
            string inputValue = txt帐号.Text.Trim(); // 获取用户输入并去除首尾空格

            if (string.IsNullOrEmpty(inputValue)) // 如果输入值为空
            {
                Lis企业列表.Items.Clear(); // 清空企业列表
                Txt密码.Clear(); // 清空密码
                txt姓名.Clear(); // 清空姓名
                txtVipID.Clear(); // 清空VIPID
                return; // 结束处理
            }
            #endregion

            #region 判断 _LoadUser.Vip_Id 是否等于输入值
            if (_LoadUser.Vip_Id == inputValue)
            {
                return; // 如果等于，直接返回
            }
            #endregion

            #region 设置查询条件并获取 VIP_Name
            // 查询条件
            string condition = $"Vip_ID = '{inputValue}' OR VIP_MOBILE = '{inputValue}'";

            // 获取 VIP_Name
            object vipNameObj = DatabaseHelper.GetColumnValue("M", "VIP_Arct", "Vip_Name", condition);

            if (vipNameObj == null) // 如果 VIP_Name 返回为空
            {
                Lis企业列表.Items.Clear(); // 清空企业列表
                Txt密码.Clear(); // 清空密码
                txt姓名.Clear(); // 清空姓名
                txtVipID.Clear(); // 清空VIPID
                return; // 结束处理
            }

            txt姓名.Text = vipNameObj.ToString(); // 将 Vip_Name 填入 txt姓名
            #endregion

            #region 获取 DataSysTemp 并处理企业列表
            object dataSysTempObj = DatabaseHelper.GetColumnValue("M", "VIP_Arct", "DataSysTemp", condition); // 获取 DataSysTemp
            if (dataSysTempObj != null) // 如果 DataSysTemp 不为空
            {
                string[] companyList = dataSysTempObj.ToString().Split('|'); // 使用 | 分隔符分割字符串

                #region 处理包含“所有企业”的情况
                if (companyList.Contains("所有企业")) // 如果企业列表中包含 "所有企业"
                {
                    //string allCompanies = DatabaseHelper.GetAllCompanies(); // 调用通用方法获取所有企业列表
                    //lis企业列表.Items.Clear(); // 清空企业列表
                    //lis企业列表.Items.AddRange(allCompanies.Split('|')); // 填充所有企业列表



                    string allCompanies = DatabaseHelper.GetAllCompanies(); // 调用通用方法获取所有企业列表
                    companyList = allCompanies.Split('|'); // 将返回的字符串用 | 分隔并赋值给 companyList

                    Lis企业列表.Items.Clear(); // 清空企业列表
                    Lis企业列表.Items.AddRange(companyList); // 填充所有企业列表





                }
                else // 如果企业列表中不包含 "所有企业"
                {
                    List<string> validCompanies = GetValidCompanies(companyList); // 获取有效企业列表
                    Lis企业列表.Items.Clear(); // 清空企业列表
                    Lis企业列表.Items.AddRange(validCompanies.ToArray()); // 填充有效企业列表
                }
                #endregion
            }
            #endregion

            #region 获取 Vip_ID 并更新 _LoadUser.Vip_Id
            // 获取 Vip_ID
            object vipIDObj = DatabaseHelper.GetColumnValue("M", "VIP_Arct", "Vip_ID", condition);
            txtVipID.Text = vipIDObj?.ToString(); // 将 Vip_ID 填入 txtVIPID

            // 更新 _LoadUser.Vip_Id
            _LoadUser.Vip_Id = vipIDObj?.ToString(); // 将 Vip_ID 更新到 _LoadUser.Vip_Id
            #endregion
        }
#endregion















        private void txt帐号_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txt帐号_KeyDown(object sender, KeyEventArgs e)
        {
                if (e.KeyCode == Keys.Enter)
                {
                    ProcessInput(); // 调用处理输入的逻辑
                }
        }

        #region 设置 cek密码 的外观和 Txt密码 的显示方式 // 定义 cek密码 和 Txt密码 的行为
        private void cek密码_CheckedChanged(object sender, EventArgs e) // 当 cek密码 状态改变时触发的事件
        {
            #region 判断 cek密码 是否选中 // 检查 cek密码 是否被选中
            if (cek密码.Checked) // 如果 cek密码 被选中
            {
                cek密码.BackColor = Color.Gray; // 设置 cek密码 的背景色为灰色
                Txt密码.Properties.PasswordChar = '*'; // 设置 Txt密码 为密文 *，
            }
            else // 如果 cek密码 未被选中
            {
                cek密码.BackColor = Color.LightBlue; // 设置 cek密码 的背景色为淡蓝色
                Txt密码.Properties.PasswordChar = '\0'; // 设置 Txt密码 为 明文，
            }
            #endregion
        }
        #endregion

        private void labelControl22_Click(object sender, EventArgs e)
        {

        }

        private void Lis企业列表_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCompany = Lis企业列表.SelectedItem?.ToString(); // 获取选择的企业名称

            if (string.IsNullOrEmpty(selectedCompany)) // 检查选择的企业是否为空
            {
                But登录.Enabled = false; // 禁用登录按钮

                // 清除以下控件的值
                Txt数据连接服务器.Text = ""; // 清除数据连接服务器文本框的值
                Txt数据连接数据库.Text = ""; // 清除数据连接数据库文本框的值
                Txt配置连接服务器.Text = ""; // 清除配置连接服务器文本框的值
                Txt配置连接数据库.Text = ""; // 清除配置连接数据库文本框的值

                return; // 返回，结束方法执行
            }

            But登录.Enabled = true; // 启用登录按钮

            // 根据选择的企业进行配置连接的初始化
            bool confConnected = ConnectionHelper.InitializeConfConnection(selectedCompany); // 初始化配置连接

            // 根据选择的企业进行数据连接的初始化
            bool dataConnected = ConnectionHelper.InitializeDataConnection(selectedCompany); // 初始化数据连接

            // 配置连接初始化成功时，赋值给相应的控件
            if (confConnected)
            {
                Txt配置连接服务器.Text = _Connection.Conf_Server; // 赋值配置连接服务器的值
                Txt配置连接数据库.Text = _Connection.Conf_Database; // 赋值配置连接数据库的值
                MessageBox.Show($"{_Connection.Data_Database}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示数据连接初始化失败的错误信息

            }
            else
            {
                MessageBox.Show("配置连接初始化失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示配置连接初始化失败的错误信息
            }

            // 数据连接初始化成功时，赋值给相应的控件
            if (dataConnected)
            {
                Txt数据连接服务器.Text = _Connection.Data_Server; // 赋值数据连接服务器的值
                Txt数据连接数据库.Text = _Connection.Data_Database; // 赋值数据连接数据库的值
                MessageBox.Show($"{_Connection.Data_Database}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示数据连接初始化失败的错误信息
            }
            else
            {
                MessageBox.Show("数据连接初始化失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示数据连接初始化失败的错误信息
            }
        }

        private void But登录_Click(object sender, EventArgs e)
        {
<<<<<<< Updated upstream
            MessageBox.Show(_Connection.Main_Str, "Main_Str 的值"); // 显示 Main_Str 的值

=======
            string 用户ID = this.txt帐号.Text;
            string 选择的企业 = this.Lis企业列表.SelectedItem?.ToString();
            string 输入密码明文 = Txt密码.Text; // 获取用户输入的密码明文

            // 检查用户ID和选择的企业是否为空
            if (string.IsNullOrEmpty(用户ID))
            {
                MessageBox.Show("用户ID不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(选择的企业))
            {
                MessageBox.Show("请选择企业", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 检查密码是否为空
            if (string.IsNullOrEmpty(输入密码明文))
            {
                MessageBox.Show("密码不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // 退出方法，不继续执行
            }

            // 调用初始化用户信息方法
            bool 初始化成功 = 中文类.初始化用户信息(用户ID, 选择的企业);

            if (初始化成功)
            {
                // 加密输入的密码
                string 输入密码暗文 = SecurityHelper.Encrypt(输入密码明文); // 将明文密码加密

                // 比较加密后的密码与用户保存的密码
                if (输入密码暗文 != _LoadUser.PassWordPC9)
                {
                    MessageBox.Show("密码错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // 退出方法，不继续执行
                }

                // 如果密码正确，继续执行
                if (_LoadUser.Is_Load) // 如果 _LoadUser.Is_Load 为 True
                {
                    // 登录系统
                    MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // 你可以在这里添加更多登录成功后的逻辑
                }
                else // 如果 _LoadUser.Is_Load 为 False
                {
                    // 退出系统
                    MessageBox.Show("用户未加载，系统将退出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); // 退出应用程序
                }
            }
            else
            {
                MessageBox.Show(_Sys.MessT, "初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 如果初始化成功，检查 _LoadUser.Is_Load 的状态
            if (_LoadUser.Is_Load) // 如果 _LoadUser.Is_Load 为 True
            {
                // 登录系统
                // 在这里添加登录系统的代码
                MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else // 如果 _LoadUser.Is_Load 为 False
            {
                // 退出系统
                MessageBox.Show("用户未加载，系统将退出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // 退出应用程序
            }
        }

        private void txtVipID_Validated(object sender, EventArgs e)
        {
            执行初始化过程(); // 调用执行初始化过程的方法
        }


        private void 执行初始化过程()
        {
            // 获取用户ID和选择的企业
            string 用户ID = this.txt帐号.Text;
            string 选择的企业 = this.Lis企业列表.SelectedItem?.ToString();

            // 检查用户ID和选择的企业是否为空
            if (string.IsNullOrEmpty(用户ID))
            {
                //MessageBox.Show("用户ID不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(选择的企业))
            {
                //MessageBox.Show("请选择企业", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 调用初始化用户信息方法
            bool 初始化成功 = 中文类.初始化用户信息(用户ID, 选择的企业);

            if (初始化成功)
            {
                // 初始化成功后的逻辑处理
                // 你可以在这里添加其他需要执行的代码
            }
            else
            {
                MessageBox.Show(_Sys.MessT, "初始化失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lis企业列表_SelectedValueChanged(object sender, EventArgs e)
        {
            执行初始化过程(); // 调用执行初始化过程的方法
>>>>>>> Stashed changes
        }
        //这里是我修改的
    }
}
