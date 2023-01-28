using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.Domain.Impl.Main;

using System;
using System.Linq;

namespace JumpsellerSync.BusinessLogic.Impl.Mappings
{
    internal class ProviderProfile : Profile
    {
        public ProviderProfile()
        {
            var providerTypeConverter = new ProviderTypeConverter();

            CreateMap<ProviderDto, BaseProvider>()
                .ConvertUsing(providerTypeConverter);

            CreateMap<ProviderDto, HourlyProvider>()
                .Ignore(provider => provider.NextSynchronization)
                .MapFrom(
                    provider => provider.Hours,
                    dto => dto.Hours.Select(hSpec => new Hour(hSpec)).ToList());

            CreateMap<ProviderDto, PeriodicallyProvider>()
               .Ignore(provider => provider.NextSynchronization)
               .MapFrom(
                    provider => provider.Interval,
                    dto => dto.Interval.Value)
               .MapFrom(
                    provider => provider.StartTime,
                    dto => TimeSpan.ParseExact(dto.StartTime, @"%h\:%m", default));



            CreateMap<BaseProvider, ProviderDto>()
                .ConvertUsing(providerTypeConverter);

            CreateMap<PeriodicallyProvider, ProviderDto>()
                .Ignore(dto => dto.Hours)
                .MapFrom(dto => dto.ProviderType, provider => ProviderType.PeriodicallyProvider)
                .MapFrom(dto => dto.StartTime, provider => provider.StartTime.ToString(@"hh\:mm"));

            CreateMap<HourlyProvider, ProviderDto>()
                .MapFrom(
                    dto => dto.Hours,
                    provider => provider.Hours.Select(h => h.Time.ToString(@"hh\:mm")).ToList())
                .MapFrom(dto => dto.ProviderType, provider => ProviderType.HourlyProvider)
                .Ignore(dto => dto.StartTime)
                .Ignore(dto => dto.Interval);
        }
    }

    internal class ProviderTypeConverter
        : ITypeConverter<ProviderDto, BaseProvider>,
          ITypeConverter<BaseProvider, ProviderDto>
    {
        public BaseProvider Convert(
            ProviderDto source, BaseProvider destination, ResolutionContext context)
        {
            destination ??= source.ProviderType switch
            {
                ProviderType.HourlyProvider => new HourlyProvider(),
                ProviderType.PeriodicallyProvider => new PeriodicallyProvider(),
                _ => throw new ArgumentException(nameof(ProviderDto.ProviderType))
            };
            var destType = source.ProviderType switch
            {
                ProviderType.HourlyProvider => typeof(HourlyProvider),
                _ => typeof(PeriodicallyProvider),
            };

            context.Mapper.Map(source, destination, typeof(ProviderDto), destType);
            return destination;
        }

        public ProviderDto Convert(
            BaseProvider source, ProviderDto destination, ResolutionContext context)
        {
            destination ??= new ProviderDto();

            context.Mapper.Map(source, destination, source.GetType(), typeof(ProviderDto));
            return destination;
        }
    }
}
