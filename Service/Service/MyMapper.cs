using AutoMapper;
using Repository.Entity;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class MyMapper:Profile
    {
        public MyMapper()
        {
            //image
            //המרה מהשרת ללקוח
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Image, src => src.MapFrom(s => convertToByte(Environment.CurrentDirectory + "/Images/" + s.ImageUrl)));
            //המרה מהלקוח לשרת
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.ImageUrl, src => src.MapFrom(s => s.File.FileName));

            //image
            //המרה מהשרת ללקוח
            CreateMap<Singer, SingerDto>()
                .ForMember(dest => dest.Image, src => src.MapFrom(s => convertToByte(Environment.CurrentDirectory + "/Images/" + s.ImageUrl)));
            //המרה מהלקוח לשרת
            CreateMap<SingerDto, Singer>()
                .ForMember(dest => dest.ImageUrl, src => src.MapFrom(s => s.File.FileName));

            //video
            //המרה משרת ללקוח
            CreateMap<Song, SongDto>()
                .ForMember(dest => dest.Video, src => src.MapFrom(s => convertToByte(Environment.CurrentDirectory + "/Video/" + s.VideoUrl)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConvertEnumToString(src.Type)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => ConvertEnumToString(src.Category)))
                .ReverseMap();

            //המרה מהלקוח לשרת
            CreateMap<SongDto, Song>()
                .ForMember(dest => dest.VideoUrl, src => src.MapFrom(s => s.File.FileName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConvertStringToEnum<Types>(src.Type)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => ConvertStringToEnum<Categories>(src.Category)));

            //המרה מהלקוח לשרת
            CreateMap<UserSongHistoryDto, UserSongHistory>();
            //המרה משרת ללקוח
            CreateMap<UserSongHistory, UserSongHistoryDto>();

            //המרה מהלקוח לשרת
            CreateMap<FeedbackDto, Feedback>();
            //המרה משרת ללקוח
            CreateMap<Feedback, FeedbackDto>();
        }
        public byte[] convertToByte(string image)
        {
            var res = System.IO.File.ReadAllBytes(image);
            return res;
        }
        private static string ConvertEnumToString<T>(T enumValue) where T : Enum
        {
            return enumValue.ToString();
        }

        private static T ConvertStringToEnum<T>(string enumString) where T : Enum
        {
            if (string.IsNullOrEmpty(enumString))
                return default; 

            string[] values = enumString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            int result = 0;
            foreach (string value in values)
            {
                if (Enum.TryParse(typeof(T), value.Trim(), true, out object enumValue))
                {
                    result |= (int)enumValue;
                }
                else
                {
                    throw new ArgumentException($"Requested value '{value}' was not found in {typeof(T).Name}.");
                }
            }

            return (T)(object)result;
        }

    }
}
