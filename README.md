anycmd
======

Anycmd.AC是完全开源免费的通用权限管理框架、系统、中间件。

###运行
------------
找到Web.config的BootDbConnString应用设置项，将这个连接字符串的密码修改成您的密码。Web.config中只有这一个引导库连接字符串，
其余数据库的连接字符串在Anycmd引导库的RDatabase表中，请使用SqlServer管理工具找到Anycmd数据库的RDatabase表修改其密码项。

###测试账户
------------
成功运行后转到“用户”模块，所有现有账户密码都是“111111”六个1。

###路线图
------------ 
* 1，书写单元测试；
* 2，书写教程；
* 3，替换掉UI层，去除试用版的miniui框架；考虑使用extjs
* 4，内置数据交换系统，用以各业务系统与中心系统间的权限数据交换；
* 5，优化性能；发布1.0版本；
* 6，放入iopenworks开放工厂；
* 7，支持SSO；
* 8，基于wf5支持工作流http://wf5.codeplex.com/
* 9，组建开源社区；
* 10，支持XACML；

###授权协议
------------
声明：本程序使用GPL协议，使用者和开发者必须遵守：

* 1、确保软件自始至终都以开放源代码形式发布，保护开发成果不被窃取用作商业发售。任何一套软 件，只要其中使用了受 GPL 协议保护的第三方软件的源程序，并向非开发人员发布时，软件本身也就自动成为受 GPL 保护并且约束的实体。也就是说，此时它必须开放源代码。
* 2、你可以去掉所有原作的版权 信息，只要你保持开源，并且随源代码、二进制版附上 GPL 的许可证就行，让后人可以很明确地得知此软件的授权信息。
* 3、无论软件以何种形式发布，都必须同时附上源代码。例如在 Web 上提供下载，就必须在二进制版本（如果有的话）下载的同一个页面，清楚地提供源代码下载的链接。如果以光盘形式发布，就必须同时附上源文件的光盘。
* 4、开发或维护遵循 GPL 协议开发的软件的公司或个人，可以对使用者收取一定的服务费用。但还是一句老话——必须无偿提供软件的完整源代码，不得将源代码与服务做捆绑或任何变相捆绑销售。
* 5、请在软件下方保留原软件的授权信息。
联系人:薛兴帅
QQ:23934360
Email:anycmd@qq.com