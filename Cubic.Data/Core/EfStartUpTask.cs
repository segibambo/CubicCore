using Cubic.Data.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cubic.Data.Core
{
    public class EfStartUpTask : IStartupTask
    {
        /// <summary>
        /// Execute
        /// </summary>
        public void Execute()
        {
            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IDataProvider>();
                if (provider == null)
                    throw new NopException("No IDataProvider found");
                provider.SetDatabaseInitializer();
            }
        }

        /// <summary>
        /// Gets order of this startup task implementation
        /// </summary>
        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
