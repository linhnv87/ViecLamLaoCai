using AutoMapper;
using Core.QueryModels;
using Database.Models;
using Database.STPCModels;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfile
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
             
            CreateMap<TblDocument, DocumentDTO>().ReverseMap();
            CreateMap<TblDocumentReview, DocumentReviewDTO>().ReverseMap();
            CreateMap<TblComment, CommentDTO>().ReverseMap();
            CreateMap<TblField, FieldDTO>().ReverseMap();
            CreateMap<TblDocumentFile, DocumentFileDTO>().ReverseMap();
            CreateMap<TblDocumentHistory, DocumentHistoryDTO>().ReverseMap();
            CreateMap<TblComment, CommentDTO>().ReverseMap();
            CreateMap<TblNotification, NotificationDTO>().ReverseMap();
            CreateMap<UserInfoDTO,AppUser>().ReverseMap();
            CreateMap<AppRole, RoleDTO>().ReverseMap();
            CreateMap<AppUserRoles, RoleDTO>();
            CreateMap<TblStatuses, CategoryStatusesDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StatusCode))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Title));
            CreateMap<TblDocumentTypes, TblDocumentTypesDTO>().ReverseMap();

            CreateMap<CfgWorkFlowVModelDTO, CfgWorkFlow>();
            CreateMap<CfgWorkFlow, ReviewOrderByIdDTO>()
                .ForMember(dest => dest.UsersDto, opt => opt.Ignore());

            CreateMap<ReviewOrderGroupDetailVModelDTO, CfgWorkFlowGroup>();
            CreateMap<CfgWorkFlowGroup, ReviewOrderGroupDetailByIdlDTO>();

            CreateMap<ReviewOrderUserDetailVModelDTO, CfgWorkFlowUser>();
            CreateMap<CfgWorkFlowUser, ReviewOrderUserDetailByIdDTO>();
            CreateMap<CfgRolesModel, CfgRolesModelDTO>();

            CreateMap<CfgUsersWorkFlowQueryModel, CfgUsersWorkFlowVModelDTO>().ReverseMap();
            CreateMap<CfgUsersWorkFlowVModelDTO, CfgWorkFlowUser>();
            CreateMap<TblDeparments, DeparmentsDTO>().ReverseMap();
            CreateMap<TblGroups, GroupDTO>().ReverseMap();
            CreateMap<TblGroupDetails, GroupDetailDTO>().ReverseMap();

            CreateMap<ReviewReportSTPC, ReviewDocumentExportDTO>();
            CreateMap<ReviewReportSTPC, ReivewDTO>();
            CreateMap<ReportApprovalByUserSTPC, ReportApprovalByUserDTO>();
            CreateMap<ReportApprovalByUserDTO, ExportApprovalByUserDTO>()
                .ForMember(dest => dest.RoleNames, opt => opt.MapFrom(src => src.Roles != null ? string.Join(", ", src.Roles.Select(r => r.Description)) : string.Empty));
            CreateMap<ReportApprovalByUserSTPC, ExportApprovalByUserDTO>();
            CreateMap<RawUserApprovalSTPC, RawUserApprovalDTO>();
            CreateMap<RawUserApprovalDTO, ExportRawUserApprovalDTO>()
                .ForMember(dest => dest.RoleNames, opt => opt.MapFrom(src => src.Roles != null ? string.Join(", ", src.Roles.Select(r => r.Description)) : string.Empty));

            CreateMap<RawUserApprovalSTPC, ExportRawUserApprovalDTO>();
            CreateMap<ReportDocumentSTPC, ReportDocumentDTO>();
            CreateMap<TblUnit, UnitDTO>().ReverseMap();
            CreateMap<ExportFileDTO, ExportFileInfoDTO>().ReverseMap();
        }
    }
}
