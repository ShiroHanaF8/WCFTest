using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceContract]
    public interface IServerClass
    {
        [OperationContract]
        int Update();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServerClass : IServerClass
    {
        int count = 0;
        public int Update()
        {
            return ++count;
        }
    }

    [ServiceContract]
    public interface IClientClass
    {
        [OperationContract]
        int CheckClient();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ClientClass : IClientClass
    {
        int count = 0;
        public int CheckClient()
        {
            return ++count;
        }
    }
}
