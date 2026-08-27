using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tour_Management.Application.Interfaces;
using Tour_Management.Application.Mappings;
using Tour_Management.Application.Services;
using Tour_Management.Application.Validators;
using Tour_Management.Application.DTOs;

namespace Tour_Management.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingService, BookingService>();

        // Validators
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<TourCreateDto>, TourCreateDtoValidator>();
        services.AddScoped<IValidator<BookingCreateDto>, BookingCreateDtoValidator>();

        return services;
    }
}
