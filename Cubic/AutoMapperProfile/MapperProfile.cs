using AutoMapper;
using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Cubic.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.AutoMapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //PortalSettingViewModel
            CreateMap<FrameworkDefaultSettingViewModel, PortalVersion>()
                .ForMember(x => x.DevelopedBy, y => { y.MapFrom(p => p.Developed); })
                .ForMember(x => x.UX, y => { y.MapFrom(p => p.Graphic); })
                .ReverseMap();
            CreateMap<PortalSettingViewModel, Application>()
                 .ForMember(x => x.ApplicationName, y => { y.MapFrom(p => p.PortalTitle); })
                .ForMember(x => x.Description, y => { y.MapFrom(p => p.PortalDescription); })
                .ForMember(x => x.HasAdminUserConfigured, y => { y.MapFrom(p => p.HasAdminUserConfigured); })
                .ForMember(x => x.TermsAndConditions, y => { y.MapFrom(p => p.TermsAndConditionPath); })
                .ForMember(x => x.IsActive, y => y.Ignore())
                .ForMember(x => x.IsDeleted, y => y.Ignore())
                .ReverseMap();
            CreateMap<AdminUserSettingViewModel,ApplicationUser>()
                .ForMember(x => x.FirstName, y => { y.MapFrom(p => p.FirstName); })
               .ForMember(x => x.LastName, y => { y.MapFrom(p => p.LastName); })
               .ForMember(x => x.MiddleName, y => { y.MapFrom(p => p.MiddleName); })
               .ForMember(x => x.FullName, y => { y.MapFrom(p => string.Format("{0},{1} {2}", p.LastName, p.FirstName, p.MiddleName)); })
               .ForMember(x => x.Email, y => { y.MapFrom(p => p.Email); })
               .ForMember(x => x.UserName, y => { y.MapFrom(p => p.UserName); })
               .ForMember(x => x.PhoneNumber, y => { y.MapFrom(p => p.PhoneNumber); })
               .ForMember(x => x.MobileNumber, y => { y.MapFrom(p => p.MobileNumber); })
               .ForMember(x => x.IsActive, y => y.Ignore())
               .ForMember(x => x.IsDeleted, y => y.Ignore())
               .ReverseMap();

            CreateMap<EmailListViewModel, EmailTemplate>()
              .ForMember(x => x.Id, y => { y.MapFrom(p => p.EmailID); })
              .ForMember(x => x.Code, y => { y.MapFrom(p => p.EmailCode); })
              .ForMember(x => x.Name, y => { y.MapFrom(p => p.EmailName); })
              .ForMember(x => x.Body, y => y.Ignore())
              .ReverseMap();


            CreateMap<PermissionViewModel, Permission>()
              .ForMember(x => x.Code, y => { y.MapFrom(p => p.PermissionCode); })
              .ForMember(x => x.Name, y => { y.MapFrom(p => p.PermissionName); })
              .ForMember(x => x.DateCreated, y => y.Ignore())
              .ForMember(x => x.DateUpdated, y => y.Ignore())
              .ReverseMap();

            CreateMap<ApplicationRoleViewModel, ApplicationRole>()
            .ForMember(x => x.Name, y => { y.MapFrom(p => p.Name); })
            .ForMember(x => x.NormalizedName, y => y.Ignore())
            .ForMember(x => x.DateCreated, y => y.Ignore())
            .ForMember(x => x.Users, y => y.Ignore())
            .ReverseMap();

            //ApplicationRoleViewModel
        }
    }
 }
