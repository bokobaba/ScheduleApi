using AutoMapper;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Models;

namespace ScheduleApi {
    public class AutoMapperProfile : Profile {
        public AutoMapperProfile() {
            CreateMap<Employee, GetEmployeeDto>();
            CreateMap<AddEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();

            CreateMap<Request, GetRequestDto>();
            CreateMap<AddRequestDto, Request>();
            CreateMap<UpdateRequestDto, Request>();
            CreateMap<AddEmployeeRequestDto, Request>();
        }
    }
}
