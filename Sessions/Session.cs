using SfRedis.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfRedis
{
    public interface Session
    {
        void Create();

        void Connect();

        void DisConnect(string v);

        void ReConnect(string v);
        
        void WriteLog(string log);

        void ReadLog(string log);

        //联动菜单
        void ContextMenu();

        //联动工具栏
        void ContextTool();

        void Command(string text);

        //获取执行历史
        List<String> History();
    }
}
