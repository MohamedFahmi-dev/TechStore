using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Newsletter;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Services;

public class NewsletterService : INewsletterService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NewsletterService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<NewsletterSubscriberDto>>> GetAllAsync()
    {
        var subs = await _uow.NewsletterSubscribers.GetTableNoTracking().ToListAsync();
        return Result<IEnumerable<NewsletterSubscriberDto>>.Ok(_mapper.Map<IEnumerable<NewsletterSubscriberDto>>(subs));
    }

    public async Task<Result<IEnumerable<NewsletterSubscriberDto>>> GetActiveAsync()
    {
        var subs = await _uow.NewsletterSubscribers.GetTableNoTracking()
            .Where(s => s.IsActive)
            .ToListAsync();
        return Result<IEnumerable<NewsletterSubscriberDto>>.Ok(_mapper.Map<IEnumerable<NewsletterSubscriberDto>>(subs));
    }

    public async Task<Result<bool>> IsSubscribedAsync(string email)
    {
        var exists = await _uow.NewsletterSubscribers.GetTableNoTracking()
            .AnyAsync(s => s.Email == email && s.IsActive);
        return Result<bool>.Ok(exists);
    }

    public async Task<Result> SubscribeAsync(string email)
    {
        var existing = await _uow.NewsletterSubscribers.GetTableNoTracking()
            .FirstOrDefaultAsync(s => s.Email == email);

        if (existing is not null)
        {
            if (existing.IsActive)
                return Error.Validation("Newsletter.AlreadySubscribed", "This email is already subscribed.");

            // Re-activate
            var tracked = await _uow.NewsletterSubscribers.FindAsync(s => s.Email == email);
            var entity = tracked.First();
            entity.IsActive = true;
            await _uow.SaveChangesAsync();
            return Result.Ok();
        }

        var subscriber = new NewsletterSubscriber { Email = email, IsActive = true };
        await _uow.NewsletterSubscribers.AddAsync(subscriber);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UnsubscribeAsync(string email)
    {
        var subs = await _uow.NewsletterSubscribers.FindAsync(s => s.Email == email && s.IsActive);
        var entity = subs.FirstOrDefault();

        if (entity is null)
            return Error.NotFound("Newsletter.NotFound", "Subscription not found.");

        entity.IsActive = false;
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entity = await _uow.NewsletterSubscribers.GetByIdAsync(id);
        if (entity is null)
            return Error.NotFound("Newsletter.NotFound", "Subscriber not found.");

        _uow.NewsletterSubscribers.Delete(entity);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }
}
