using System;
using System.Collections.Generic;
using System.Text;

namespace Eshop.Infrastructure.Options
{
    public class RedisOptions
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName {  get; set; } = string.Empty;
    }
}
