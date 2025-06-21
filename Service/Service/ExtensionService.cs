using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Repository.Entity;
using Repository.Interface;
using Repository.Repository;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public static class ExtensionService
    {
        public static IServiceCollection AddServiceExtension(this IServiceCollection services)
        {
            services.AddRepositoryExtension();
            services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IService<SongDto>, SongService>();
            services.AddScoped<ISongService, ExtensionSongService>();
            services.AddScoped<IService<SingerDto>, SingerService>();
            services.AddScoped<IService<UserSongHistoryDto>, UserSongHistoryService>();
            services.AddScoped<IService<FeedbackDto>, FeedbackService>();
            services.AddScoped<ISingerService, ExtensionSingerService>();
            services.AddScoped<IUserService, ExtensionUserService>();
            services.AddScoped<ISpeechToTextService, SpeechToTextService>();
            services.AddScoped<ISongElasticSearchService, SongElasticSearchService>();
            services.AddScoped<IFeedbackService, ExtensionFeedbackService>();
            services.AddScoped<IExtensionHistoryService, ExtensionHistoryService>();

            services.AddAutoMapper(typeof(MyMapper));
            return services;
        }
    }
}
