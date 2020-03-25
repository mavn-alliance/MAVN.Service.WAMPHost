using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.WampHost.Domain.Services;
using WampSharp.Core.Serialization;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Rpc;

namespace Lykke.Service.WampHost.DomainServices.RpcOperations
{
    // NOTE: Not used.
    public class InitializeOperation : SyncLocalRpcOperation
    {
        private readonly ISessionCache _sessionCache;
        private readonly IBalanceUpdatePublisher _balanceUpdatePublisher;
        
        public InitializeOperation(
            ISessionCache sessionCache,
            IBalanceUpdatePublisher balanceUpdatePublisher)
            : base("com.init")
        {
            _sessionCache = sessionCache;
            _balanceUpdatePublisher = balanceUpdatePublisher;
        }

        public override RpcParameter[] Parameters => new RpcParameter[]
        {
            new RpcParameter(name: "topic", type: typeof (string), position: 0)
        };

        public override bool HasResult => true;
        
        //todo: figure out what is this property for
        public override CollectionResultTreatment CollectionResultTreatment => CollectionResultTreatment.SingleValue;

        protected override object InvokeSync<TMessage>(
            IWampRawRpcOperationRouterCallback caller,
            IWampFormatter<TMessage> formatter,
            InvocationDetails details,
            TMessage[] arguments,
            IDictionary<string, TMessage> argumentsKeywords,
            out IDictionary<string, object> outputs)
        {
            try
            {
                object[] parameters =
                    UnpackParameters(formatter, arguments, argumentsKeywords)
                        .ToArray();

                var topic = (string) parameters[0];

                var customerId = _sessionCache.GetClientId(details.AuthenticationId);

                if (topic == Topic.Balance)
                {
                    Task.Run(async () => { await _balanceUpdatePublisher.PublishAsync(customerId); });
                }

                outputs = null;

                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToJson());
                throw;
            }
        }
    }
}
