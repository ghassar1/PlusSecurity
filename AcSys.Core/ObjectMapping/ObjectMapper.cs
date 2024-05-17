using AutoMapper;

namespace AcSys.Core.ObjectMapping
{
    public static class ObjectMapper
    {
        //private static MapperConfiguration MapperConfig { get; set; }

        public static TDest Map<TSource, TDest>(TSource source)
        {
            //IMapper mapper = MapperConfig.CreateMapper();
            //TDest dest = mapper.Map<TSource, TDest>(source);

            TDest dest = Mapper.Map<TSource, TDest>(source);
            return dest;
        }

        public static TDest Map<TDest>(object source)
            where TDest : new()
        {
            //IMapper mapper = MapperConfig.CreateMapper();
            //TDest dest = mapper.Map<TDest>(source);

            TDest dest = Mapper.Map<TDest>(source);
            return dest;
        }

        public static TDest Map<TSource, TDest>(TSource source, TDest dest)
            where TSource : new()
            where TDest : new()
        {
            //IMapper mapper = MapperConfig.CreateMapper();
            //mapper.Map<TSource, TDest>(source, dest);

            Mapper.Map<TSource, TDest>(source, dest);
            return dest;
        }

        //public static TDest DynamicMap<TSource, TDest>(TSource source, TDest dest)
        //    where TSource : new()
        //    where TDest : new()
        //{
        //    Mapper.DynamicMap<TSource, TDest>(source, dest);
        //    return dest;
        //}

        //public static TDest DynamicMap<TSource, TDest>(TSource source)
        //    where TDest : new()
        //{
        //    TDest dest = Mapper.DynamicMap<TSource, TDest>(source);
        //    return dest;
        //}

        public static void ConfigureAutoMapperProfile<TProfile>() where TProfile : Profile, new()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<TProfile>(); });

            //Mapper.AssertConfigurationIsValid();

            // v4.2.0
            //var config = new MapperConfiguration(cfg => { cfg.AddProfile<AutoMapperProfile>(); });
            //config.AssertConfigurationIsValid();
            //MapperConfig = config;
        }
    }
}
