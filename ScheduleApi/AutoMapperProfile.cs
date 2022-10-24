using AutoMapper;
using ScheduleApi.Dtos.AvailabilityDtos;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Dtos.RuleGroupDtos;
using ScheduleApi.Dtos.ScheduleDtos;
using ScheduleApi.Dtos.ShiftDtos;
using ScheduleApi.Models;

namespace ScheduleApi {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<Employee, GetEmployeeDto>();
            CreateMap<Employee, GetEmployeeInfoDto>();
            CreateMap<AddEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();

            CreateMap<Request, GetRequestDto>();
            CreateMap<AddRequestDto, Request>();
            CreateMap<UpdateRequestDto, Request>();

            CreateMap<Schedule, GetScheduleDto>();
            CreateMap<AddScheduleDto, Schedule>();
            CreateMap<UpdateScheduleDto, Schedule>();

            CreateMap<RuleGroup, GetRuleGroupDto>();
            CreateMap<SaveRuleGroupDto, RuleGroup>();

            CreateMap<Shift, GetShiftDto>();
            CreateMap<UpdateShiftDto, Shift>();
            CreateMap<AddShiftDto, Shift>();

            CreateMap<Availability, GetAvailabilityDto>();
            CreateMap<UpdateAvailabilityDto, Availability>();
        }
    }
}
