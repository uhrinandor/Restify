using AutoMapper;
using RestifyServer.TypeContracts;

namespace RestifyServer.Configuration;

public static class MapperConfigFactory
{
    public static void CreateMapperConfiguration(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Models.Entity, Entity>();
        cfg.CreateMap<Models.Admin, Admin>().IncludeBase<Models.Entity, Entity>();
        cfg.CreateMap<Models.Waiter, Waiter>().IncludeBase<Models.Entity, Entity>();
        cfg.CreateMap<Models.Category, Category>().IncludeBase<Models.Entity, Entity>();
        cfg.CreateMap<Models.Category, NestedCategory>().IncludeBase<Models.Entity, Entity>();
        cfg.CreateMap<Models.Product, Product>().IncludeBase<Models.Entity, Entity>();
        cfg.CreateMap<Models.Table, Table>().IncludeBase<Models.Entity, Entity>();
    }
}
