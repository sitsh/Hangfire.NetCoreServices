using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;


namespace Hangfire.NetCoreServices.Impl
{
    public class CosJob: ICosJob
    {
       
        public CosJob()
        {
            //构造函数 注入 业务服务接口
        }
        
        //[Queue("critical")] 配置 入队队列名称 默认 default
        public  void UpCosJobRun(CosParams para)
        {
            Log.Information("CosJob running ...mid:{0},filePath:{1}", para.Mid, para.Name);
        }
    }

    public interface ICosJob
    {
        [AutomaticRetry(Attempts = 10)]//配置重试策略 默认10
        //[Queue("critical")] //配置 入队队列名称 默认 default
        void UpCosJobRun(CosParams para);
    }

    public class CosParams
    {
        public string Mid { set; get; }
        public string Name { set; get; }
    }
}
