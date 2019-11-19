using System;
using System.Linq;
using AutoMapper;
using DatingApp.Dtos;
using DatingApp.Models;

namespace DatingApp.helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                  {
                      opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                  })
                .ForMember(dest => dest.Age, opt =>
                 {
                     opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                 });


            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                  {
                      opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                  })
                .ForMember(dest => dest.Age, opt =>
                 {
                     opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                 });


            CreateMap<Photo, PhotoForDetailDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreateDto, Photo>();
            CreateMap<PhotoForCreateDto, Photo>();
            CreateMap<MessageCreateDto, Message>().ReverseMap();
            CreateMap<Message, MessageReturnDto>()
                .ForMember(m => m.SenderPhotoUrl,
                opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl,
                opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}
