using System; // 引用系统命名空间
using System.Data; // 引用数据命名空间
using System.Data.SqlClient; // 引用SQL客户端命名空间
using System.Windows.Forms; // 引用Windows窗体命名空间
using System.IO; // 引用IO命名空间，用于文件操作
using System.Security.Cryptography; // 引用安全加密命名空间
using System.Text; // 引用文本处理命名空间
using System.Collections.Generic; // 引用 List<T> 泛型类 系统泛型命名空间

namespace CMID9_DEV.Models // 命名空间
{
    #region 静态类DatabaseHelper
    public static class DatabaseHelper // 定义一个静态类DatabaseHelper
    {
        #region 传递条件获取指定列的值 GetColumnValue 的结果作为 DataTable 使用
        public static DataTable GetColumnValueAsDataTable(string connectionScheme, string tableName, string columnName, string condition)
        {
            string connectionString = "";

            if (connectionScheme == "M")
            {
                connectionString = _Connection.Main_Str;
            }
            else if (connectionScheme == "C")
            {
                connectionString = _Connection.Conf_Str;
            }
            else if (connectionScheme == "D")
            {
                connectionString = _Connection.Data_Str;
            }
            else
            {
                MessageBox.Show("缺少服务器连接方案", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            string query = $"SELECT {columnName} FROM {tableName} WHERE {condition}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable; // 返回DataTable而不是object
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("数据库连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 传递条件获取指定列的值
        public static object GetColumnValue(string connectionScheme, string tableName, string columnName, string condition) // 定义一个公共静态方法GetColumnValue，传入连接方案、表名称、列名称和条件
        {
            string connectionString = ""; // 初始化连接字符串为空

            // 判断连接方案，选择合适的连接字符串
            if (connectionScheme == "M") // 如果连接方案为M（系统连接）
            {
                connectionString = _Connection.Main_Str; // 使用系统连接字符串
            }
            else if (connectionScheme == "C") // 如果连接方案为C（配置连接）
            {
                connectionString = _Connection.Conf_Str; // 使用配置连接字符串
            }
            else if (connectionScheme == "D") // 如果连接方案为D（数据连接）
            {
                connectionString = _Connection.Data_Str; // 使用数据连接字符串
            }
            else // 如果连接方案不匹配
            {
                MessageBox.Show("缺少服务器连接方案", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出错误提示框
                return null; // 返回null
            }

            // 构建SQL查询语句
            string query = $"SELECT {columnName} FROM {tableName} WHERE {condition}"; // 构建查询语句

            using (SqlConnection connection = new SqlConnection(connectionString)) // 使用using语句创建SQL连接，自动释放资源
            {
                using (SqlCommand command = new SqlCommand(query, connection)) // 使用using语句创建SQL命令，自动释放资源
                {
                    try // 尝试执行以下代码
                    {
                        connection.Open(); // 打开数据库连接
                        object result = command.ExecuteScalar(); // 执行查询并返回单个结果

                        if (result == null) // 如果未找到数据
                        {
                            MessageBox.Show($"没有找到匹配的数据 \r \n {tableName}  \n {columnName} \n  {condition}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); // 弹出提示框
                        }

                        return result; // 返回查询结果
                    }
                    catch (Exception ex) // 如果发生异常
                    {
                        MessageBox.Show("数据库连接失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出错误提示框
                        return null; // 返回null
                    }
                    finally // 无论是否发生异常，最终执行
                    {
                        connection.Close(); // 关闭数据库连接
                    }
                }
            }
        }
        #endregion

        #region 获取所有企业列表方法
        public static string GetAllCompanies() // 获取所有企业列表的方法开始
        {
            DataTable resultTable = GetColumnValueAsDataTable("M", "App_Register", "DISTINCT CompanyAbbr", "1=1") as DataTable; // 从数据库中获取所有不重复的企业缩写

            if (resultTable != null && resultTable.Rows.Count > 0) // 如果查询结果不为空且有数据
            {
                List<string> companyList = new List<string>(); // 用于存储企业缩写的列表

                foreach (DataRow row in resultTable.Rows) // 遍历结果集中的每一行
                {
                    companyList.Add(row["CompanyAbbr"].ToString()); // 将每个企业缩写添加到列表中
                }

                return string.Join("|", companyList); // 将列表中的企业缩写用 '|' 分隔符连接成字符串并返回
            }

            return string.Empty; // 如果查询结果为空，返回空字符串
        }
        #endregion
    }
    #endregion

    #region 静态类SecurityHelper
    public static class SecurityHelper // 定义一个静态类SecurityHelper
    {
        private static readonly string EncryptionKey = _Sys.PassKey; // 使用全局变量中的加密密钥

        #region 加密字符串
        public static string Encrypt(string plainText) // 定义一个公共静态方法Encrypt，用于加密字符串
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText); // 将明文字符串转换为字节数组
            using (Aes aes = Aes.Create()) // 使用AES创建加密对象
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32)); // 使用密钥并填充到32位
                aes.IV = new byte[16]; // 初始化向量为零
                using (MemoryStream memoryStream = new MemoryStream()) // 使用内存流
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write)) // 使用加密流
                    {
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length); // 将字节数组写入加密流
                        cryptoStream.FlushFinalBlock(); // 刷新缓冲区
                        return Convert.ToBase64String(memoryStream.ToArray()); // 返回加密后的字符串
                    }
                }
            }
        }
        #endregion

        #region 解密字符串
        public static string Decrypt(string cipherText) // 定义一个公共静态方法Decrypt，用于解密字符串
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText); // 将加密字符串转换为字节数组
            using (Aes aes = Aes.Create()) // 使用AES创建解密对象
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32)); // 使用密钥并填充到32位
                aes.IV = new byte[16]; // 初始化向量为零
                using (MemoryStream memoryStream = new MemoryStream()) // 使用内存流
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write)) // 使用解密流
                    {
                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length); // 将字节数组写入解密流
                        cryptoStream.FlushFinalBlock(); // 刷新缓冲区
                        return Encoding.UTF8.GetString(memoryStream.ToArray()); // 返回解密后的字符串
                    }
                }
            }
        }
        #endregion
    }
    #endregion

    #region ConnectionHelper 类
    public static class ConnectionHelper // 定义一个静态类ConnectionHelper
    {
        #region 系统连接的初始化  InitializeMainConnection 方法 
        public static bool InitializeMainConnection() // 定义一个公共静态方法InitializeMainConnection，返回布尔类型
        {
            #region 检查 _Connection.Main_Str 是否为空
            if (!string.IsNullOrEmpty(_Connection.Main_Str)) // 如果 _Connection.Main_Str 不为空
            {
                try // 尝试连接数据库
                {
                    _Connection.Main_Str = _Connection.Main_Str.Replace("Provider=SQLOLEDB.1;", ""); // 将"Provider="及其值删除
                    using (SqlConnection connection = new SqlConnection(_Connection.Main_Str)) // 使用SQL连接字符串创建连接
                    {
                        connection.Open(); // 打开数据库连接
                        return true; // 如果连接成功，返回True
                    }
                }
                catch (SqlException ex) // 如果连接失败，捕获SQL异常
                {
                    MessageBox.Show($"系统连接失败。\n类型: {_Connection.Main_Type}\n主机: {_Connection.Main_Server}\n数据库名称: {_Connection.Main_Database}\n用户: {_Connection.Main_UserID}\n密码: ********\n端口: {_Connection.Main_Port}\n错误信息: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出提示对话框，显示错误信息
                    _Connection.Main_Str = ""; // 将 _Connection.Main_Str 赋值为空字符串
                    return false; // 返回False
                }
            }
            #endregion

            #region _Connection.Main_Str 为空，检查 _Connection.Main_File 文件是否存在
            if (!File.Exists(_Connection.Main_File)) // 如果 _Connection.Main_File 文件不存在
            {
                MessageBox.Show($"系统连接配置文件不存在，无法进行系统连接。\n文件路径和名称: {_Connection.Main_File}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出提示对话框，提示文件不存在
                _Connection.Main_Str = ""; // 将 _Connection.Main_Str 赋值为空字符串
                return false; // 返回False
            }
            #endregion

            try // 尝试读取并解密连接字符串
            {
                #region 读取并解密连接字符串
                string encryptedConnectionString = File.ReadAllText(_Connection.Main_File); // 从文件中读取加密的系统连接字符串
                _Connection.Main_Str = SecurityHelper.Decrypt(encryptedConnectionString); // 解密后赋值给全局变量 _Connection.Main_Str
                #endregion

                #region 尝试用解密后的连接字符串连接数据库
                _Connection.Main_Str = _Connection.Main_Str.Replace("Provider=SQLOLEDB.1;", ""); // 将"Provider="及其值删除
                using (SqlConnection connection = new SqlConnection(_Connection.Main_Str)) // 使用解密后的连接字符串创建连接
                {
                    connection.Open(); // 打开数据库连接
                }
                #endregion

                #region 解析连接字符串并赋值全局变量
                try
                {
                    _Connection.Main_Server = ParseConnectionString(_Connection.Main_Str, "server"); // 解析并赋值系统连接主机的IP或主机名
                    _Connection.Main_Database = ParseConnectionString(_Connection.Main_Str, "database"); // 解析并赋值系统连接的数据库名称
                    _Connection.Main_UserID = ParseConnectionString(_Connection.Main_Str, "userid"); // 解析并赋值系统连接数据库用户的用户名
                    _Connection.Main_Password = ParseConnectionString(_Connection.Main_Str, "password"); // 解析并赋值系统连接数据库用户的密码
                    _Connection.Main_Port = ParseConnectionString(_Connection.Main_Str, "data_port"); // 解析并赋值系统连接的端口号
                    _Connection.Main_Type = ParseConnectionString(_Connection.Main_Str, "data_type"); // 解析并赋值系统连接的数据库类型

                    return true; // 如果解析成功且连接成功，返回True
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"解析连接字符串时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出提示对话框，显示错误信息
                    return false; // 返回False
                }
                #endregion

            }
            catch (SqlException ex) // 如果连接失败，捕获SQL异常
            {
                #region 连接失败处理
                MessageBox.Show($"系统连接失败。\n语句: {_Connection.Main_Str}\n主机: {_Connection.Main_Server}\n数据库名称: {_Connection.Main_Database}\n用户: {_Connection.Main_UserID}\n密码: ********\n端口: {_Connection.Main_Port}\n错误信息: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 弹出提示对话框，显示错误信息
                _Connection.Main_Str = ""; // 将 _Connection.Main_Str 赋值为空字符串
                return false; // 返回False
                #endregion
            }
        }
        #endregion

        #region 配置连接的初始化 InitializeConfConnection 方法
        public static bool InitializeConfConnection(string selectedCompany)
        {
            string tableName = "Connection"; // 表名
            string columnName = "Connection_PC9"; // 需要返回的列名
            string condition = $"CompanyAbbr = '{selectedCompany}' AND ServerType = 'PC9' AND ServerName = 'PC9配置连接'"; // 查询条件

            object result = DatabaseHelper.GetColumnValue("M", tableName, columnName, condition); // 调用方法获取结果

            if (result == null) // 如果结果为空
            {
                _Connection.Conf_Str = ""; // 配置连接字符串设为空
                return false; // 返回FALSE
            }

            string decryptedConnectionString = SecurityHelper.Decrypt(result.ToString()); // 解密查询结果

            try // 尝试进行连接
            {
                decryptedConnectionString = decryptedConnectionString.Replace("Provider=SQLOLEDB.1;", ""); // 将"Provider="及其值删除
                using (SqlConnection connection = new SqlConnection(decryptedConnectionString)) // 创建SQL连接
                {
                    connection.Open(); // 打开连接
                }

                // 解析连接字符串并赋值全局变量
                try
                {
                    // 解析并赋值数据连接主机的IP或主机名
                    _Connection.Conf_Server = ParseConnectionString(decryptedConnectionString, "server");
                    if (string.IsNullOrEmpty(_Connection.Conf_Server)) return true; // 如果第一个值为空，直接返回
                    // 解析并赋值数据连接的数据库名称
                    _Connection.Conf_Database = ParseConnectionString(decryptedConnectionString, "database");
                    if (string.IsNullOrEmpty(_Connection.Conf_Database)) return true; // 如果第二个值为空，直接返回
                    // 解析并赋值数据连接数据库用户的用户名
                    _Connection.Conf_UserID = ParseConnectionString(decryptedConnectionString, "userid");
                    if (string.IsNullOrEmpty(_Connection.Conf_UserID)) return true; // 如果第三个值为空，直接返回
                    // 解析并赋值数据连接数据库用户的密码
                    _Connection.Conf_Password = ParseConnectionString(decryptedConnectionString, "password");
                    if (string.IsNullOrEmpty(_Connection.Conf_Password)) return true; // 如果第四个值为空，直接返回
                    // 解析并赋值数据连接的端口号
                    _Connection.Conf_Port = ParseConnectionString(decryptedConnectionString, "data_port");
                    if (string.IsNullOrEmpty(_Connection.Conf_Port)) return true; // 如果第五个值为空，直接返回
                    // 解析并赋值数据连接的数据库类型
                    _Connection.Conf_Type = ParseConnectionString(decryptedConnectionString, "data_type");
                    if (string.IsNullOrEmpty(_Connection.Conf_Type)) return true; // 如果第六个值为空，直接返回
                    return true; // 返回True，表示连接成功 
                }
                catch (Exception ex) // 解析连接字符串时发生错误
                {
                    MessageBox.Show($"解析配置连接字符串时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误信息
                    return false; // 返回False
                }
            }
            catch (SqlException ex) // 如果连接失败
            {
                MessageBox.Show($"配置连接失败。\n错误信息: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误信息
                return false; // 返回FALSE
            }
        }
        #endregion

        #region 数据连接的初始化 InitializeDataConnection 方法
        public static bool InitializeDataConnection(string selectedCompany)
        {
            string tableName = "Connection"; // 表名
            string columnName = "Connection_PC9"; // 需要返回的列名
            string condition = $"CompanyAbbr = '{selectedCompany}' AND ServerType = 'PC9' AND ServerName = 'PC9配置连接'"; // 查询条件

            object result = DatabaseHelper.GetColumnValue("M", tableName, columnName, condition); // 调用方法获取结果

            if (result == null) // 如果结果为空
            {
                _Connection.Data_Str = ""; // 数据连接字符串设为空
                return false; // 返回FALSE
            }

            string decryptedConnectionString = SecurityHelper.Decrypt(result.ToString()); // 解密查询结果

            try // 尝试进行连接
            {
                decryptedConnectionString = decryptedConnectionString.Replace("Provider=SQLOLEDB.1;", ""); // 将"Provider="及其值删除
                using (SqlConnection connection = new SqlConnection(decryptedConnectionString)) // 创建SQL连接
                {
                    connection.Open(); // 打开连接
                }

                // 解析连接字符串并赋值全局变量
                try
                {
                    // 解析并赋值数据连接主机的IP或主机名
                    _Connection.Data_Server = ParseConnectionString(decryptedConnectionString, "server");
                    if (string.IsNullOrEmpty(_Connection.Data_Server)) return true; // 如果第一个值为空，直接返回
                    // 解析并赋值数据连接的数据库名称
                    _Connection.Data_Database = ParseConnectionString(decryptedConnectionString, "database");
                    if (string.IsNullOrEmpty(_Connection.Data_Database)) return true; // 如果第二个值为空，直接返回
                    // 解析并赋值数据连接数据库用户的用户名
                    _Connection.Data_UserID = ParseConnectionString(decryptedConnectionString, "userid");
                    if (string.IsNullOrEmpty(_Connection.Data_UserID)) return true; // 如果第三个值为空，直接返回
                    // 解析并赋值数据连接数据库用户的密码
                    _Connection.Data_Password = ParseConnectionString(decryptedConnectionString, "password");
                    if (string.IsNullOrEmpty(_Connection.Data_Password)) return true; // 如果第四个值为空，直接返回
                    // 解析并赋值数据连接的端口号
                    _Connection.Data_Port = ParseConnectionString(decryptedConnectionString, "data_port");
                    if (string.IsNullOrEmpty(_Connection.Data_Port)) return true; // 如果第五个值为空，直接返回
                    // 解析并赋值数据连接的数据库类型
                    _Connection.Data_Type = ParseConnectionString(decryptedConnectionString, "data_type");
                    if (string.IsNullOrEmpty(_Connection.Data_Type)) return true; // 如果第六个值为空，直接返回
                    return true; // 返回True，表示连接成功
                }
                catch (Exception ex) // 解析连接字符串时发生错误
                {
                    MessageBox.Show($"解析数据连接字符串时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误信息
                    return false; // 返回False
                }
            }
            catch (SqlException ex) // 如果连接失败
            {
                MessageBox.Show($"数据连接失败。\n错误信息: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误信息
                return false; // 返回FALSE
            }
        }
        #endregion

        #region 解析连接字符串并返回指定信息的全局方法
        public static string ParseConnectionString(string connectionString, string returnType) // 定义解析连接字符串并返回指定信息的全局方法
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString); // 使用SqlConnectionStringBuilder解析连接字符串

                string result = ""; // 初始化返回结果为空字符串

                // 根据需要返回的内容类型进行赋值
                switch (returnType.ToLower()) // 使用ToLower()确保不区分大小写
                {
                    case "server":
                        result = builder.DataSource; // 赋值为数据连接主机的IP或主机名
                        break;

                    case "database":
                        result = builder.InitialCatalog; // 赋值为数据连接的数据库名称
                        break;

                    case "userid":
                        result = builder.UserID; // 赋值为数据连接数据库用户的用户名
                        break;

                    case "password":
                        result = builder.Password; // 赋值为数据连接数据库用户的密码
                        break;

                    case "data_port":
                        if (builder.DataSource.Contains(",")) // 检查DataSource是否包含端口信息
                        {
                            var serverParts = builder.DataSource.Split(','); // 将服务器名称和端口分离
                            result = serverParts[1]; // 返回端口号
                        }
                        else if (builder.ContainsKey("Port"))
                        {
                            result = builder["Port"].ToString(); // 从连接字符串中获取端口号
                        }
                        else
                        {
                            result = "1433"; // 默认端口号为1433
                        }
                        break;

                    case "data_type":
                        if (connectionString.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            result = "MySQL"; // 如果字符串包含"MySql"，则返回数据库类型为MySQL
                        }
                        else if (connectionString.IndexOf("SQL Server", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            result = "SQL Server"; // 如果字符串包含"SQL Server"，则返回数据库类型为SQL Server
                        }
                        else
                        {
                            result = "无"; // 如果没有匹配的类型，返回"无"
                        }
                        break;

                    default:
                        result = ""; // 如果输入的返回类型不匹配，返回空字符串
                        break;
                }

                return result; // 返回解析的结果
            }
            catch (Exception ex) // 如果解析连接字符串时发生错误
            {
                MessageBox.Show($"解析连接字符串时发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); // 显示错误信息
                return ""; // 返回空字符串
            }
        }
        #endregion
    }
    #endregion
}
