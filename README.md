# Hangfire  use Topshelf at NetCore winservice

> 项目模板描述: 
项目通过 [Topshelf 官方示例](https://github.com/Topshelf/Topshelf/tree/develop/src/SampleTopshelfService) 代码 加入并集成了 Hangfire.AspNetCore 任务系统。最后通过 toeshelf 把 netcore App(Hangfire)安装在windows 环境下 作为win服务运行。

[Demo template ](https://github.com/sitsh/Hangfire.NetCoreServices)


## 整体结构
Hangfire --使用 sqlserver 作为存储job 引擎
ASP.NET CORE WEB APP OR API --项目中 集成 Hangfire Client 
WinServices(Core runtime) --  运行 Hangfire 任务和 Hangfire 仪表板 (上面的模板项目就是这个服务)
加入 健康监控   https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks 可以使用 HealthChecksUI 显示服务状态

## 环境运行时
* NetCore Console-- NetCore 2.2.3
* TopShelf --4.2.0
* Hangfire.AspNetCore 
* Hangfire.SqlServer 
* Microsoft.AspNetCore.App 
* Serilog
* Topshelf.Serilog

### 更新
* core sdk up 2.2.3
* add AspNetCore.HealthChecks.UI.Client 添加 健康监控 
