运行：找到Web.config的BootDbConnString应用设置项，将这个连接字符串的密码修改成您的密码。Web.config中只有这一个引导库连接字符串，
      其余数据库的连接字符串在Anycmd引导库的RDatabase表中，请使用SqlServer管理工具找到Anycmd数据库的RDatabase表修改其密码项。

测试账户： 成功运行后转到“用户”模块，所有现有账户密码都是“111111”六个1。