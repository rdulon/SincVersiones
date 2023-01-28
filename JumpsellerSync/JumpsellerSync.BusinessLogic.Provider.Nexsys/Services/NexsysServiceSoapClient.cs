using System.ServiceModel;
using System.ServiceModel.Description;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Services
{
    public partial class NexsysServiceSoapClient
    {
        static partial void ConfigureEndpoint(
            ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials)
        {
            if (serviceEndpoint.Binding is HttpBindingBase httpBinding)
            {
                httpBinding.MaxBufferSize = int.MaxValue;
                httpBinding.MaxReceivedMessageSize = int.MaxValue;
            }
        }
    }
}
