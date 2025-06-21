using Microsoft.Extensions.DependencyInjection;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public static class ExtensionRepository
    {
        public static IServiceCollection AddRepositoryExtension(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Song>, SongRepository>();
            services.AddScoped<ISongRepository, ExtensionSongRepository>();
            services.AddScoped<IRepository<Singer>, SingerRepository>();
            services.AddScoped<IRepository<UserSongHistory>, UserSongHistoryRepository>();
            services.AddScoped<IRepository<Feedback>, FeedbackRepository>();
            services.AddScoped<ISingerRepository, ExtensionSingerRepository>();
            services.AddScoped<IUserRepository, ExtensionUserRepository>();
            services.AddScoped<ISpeechToTextRepository, SpeechToTextRepository>();
            services.AddScoped<ISongElasticSearchRepository, SongElasticSearchRepository>();
            services.AddScoped<IFeedbackRepository, ExtensionFeedbackRepository>();
            services.AddScoped<IExtensionHistoryRepository, ExtensionHistoryRepository>();
            services.AddScoped<EmailService>();

            return services;
        }
    }
}
