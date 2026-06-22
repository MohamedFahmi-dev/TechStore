using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Newsletter;

namespace TechStore.BLL.Interface;

public interface INewsletterService
{
    Task<Result<IEnumerable<NewsletterSubscriberDto>>> GetAllAsync();
    Task<Result<IEnumerable<NewsletterSubscriberDto>>> GetActiveAsync();
    Task<Result> SubscribeAsync(string email);
    Task<Result> UnsubscribeAsync(string email);
    Task<Result<bool>> IsSubscribedAsync(string email);
    Task<Result> DeleteAsync(int id);
}

