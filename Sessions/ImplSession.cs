using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfRedis.Sessions
{
    class ImplSession : Session
    {
        public void Command()
        {
            throw new NotImplementedException();
        }

        virtual public void Command(string text) { }


        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void ContextMenu()
        {
            throw new NotImplementedException();
        }

        public void ContextTool()
        {
            throw new NotImplementedException();
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        virtual public void DisConnect()
        {
          
        }

        virtual public string GetIdentifier() {
            return "";
        }

        public List<string> History()
        {
            throw new NotImplementedException();
        }

        public void ReadLog(string log)
        {
            throw new NotImplementedException();
        }

        virtual public void ReConnect()
        {
        }

        virtual public void Refresh(Session session) { }

        public void WriteLog(string log)
        {
            throw new NotImplementedException();
        }

        public virtual void GetConnect() { }
    }
}
