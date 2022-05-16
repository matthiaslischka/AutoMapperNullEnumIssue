using System;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoMapperNullEnumIssue;

public class SomeSourceDto
{
    public SomeSourceEnum? SomeEnum { get; set; }
}

public class SomeDestinationDto
{
    public SomeDestinationEnum? SomeEnum { get; set; }
}

public enum SomeSourceEnum
{
    First = 1
}

public enum SomeDestinationEnum
{
    First = 1
}

public class SomeProfile : Profile
{
    public SomeProfile()
    {
        CreateMap<SomeSourceDto, SomeDestinationDto>()
            .ForMember(
                dst => dst.SomeEnum,
                opt => opt.MapFrom(
                    src => Enum.IsDefined(typeof(SomeDestinationEnum), src.SomeEnum)
                        ? src.SomeEnum
                        : null));

        CreateMap<SomeSourceEnum, SomeDestinationEnum>().ConvertUsingEnumMapping(opt => opt.MapByName());
    }
}

[TestClass]
public class UnitTest1
{
    private readonly IMapper _mapper;

    public UnitTest1()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(typeof(SomeProfile));
            cfg.EnableEnumMappingValidation();
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    [TestMethod]
    public void TestMethod1()
    {
        var someSourceDto = new SomeSourceDto { SomeEnum = 0 };
        var someDestinationDto = _mapper.Map<SomeDestinationDto>(someSourceDto);
        Assert.IsNull(someDestinationDto.SomeEnum);
    }
}