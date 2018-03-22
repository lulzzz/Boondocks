// using Boondocks.Device.Domain.Entities;
// using Dapper;
// using Dapper.FluentMap;
// using Dapper.FluentMap.Dommel;
// using Dapper.FluentMap.Dommel.Mapping;
// using Dapper.FluentMap.Mapping;
// using NetFusion.Bootstrap.Plugins;

// namespace Boondocks.Device.Infra.Mappings
// {

//     public class MappingModule : PluginModule
//     {
//         public override void Configure()
//         {
//             FluentMapper.Initialize(config => {
//                 config.AddMap(new LogEventMap());
//                 config.ForDommel();
//             });
//         }

//         private class LogEventMap : DommelEntityMap<ApplicationLog>
//         {
//             public LogEventMap()
//             {
//                 this.Map(e => e.Id).
//             }
//         }
//     }
// }