﻿using ClinicPlatformDatabaseObject;
using ClinicPlatformDTOs.BookingModels;
using ClinicPlatformObjects.BookingModels;
using ClinicPlatformRepositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ClinicPlatformRepositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DentalClinicPlatformContext context;
        private bool disposedValue;

        public BookingRepository(DentalClinicPlatformContext context)
        {
            this.context = context;
        }

        public AppointmentInfoModel CreateNewBooking(AppointmentInfoModel booking)
        {
            var appointment = MapBookingModelToBooking(booking);
            context.Appointments.Add(appointment);

            return MapBookingToBookingModel(appointment);
        }

        public AppointmentInfoModel? GetBooking(Guid id)
        {
            var result = context.Appointments.Find(id);
            return result == null ? null : MapBookingToBookingModel(result);
        }

        public IEnumerable<AppointmentInfoModel> GetAll()
        {
            return from item in context.Appointments select MapBookingToBookingModel(item);
        }

        public BookedServiceInfoModel? AddBookingService(BookedServiceInfoModel bookedService)
        {
            var service = ToBookedService(bookedService);
            context.BookedServices.Add(service);

            return ToBookedServiceModel(service);
        }

        public BookedServiceInfoModel? GetBookingService(Guid appointmentId)
        {
            var bookedService = context.BookedServices.Find(appointmentId);

            if (bookedService != null)
            {
                return ToBookedServiceModel(bookedService);
            }

            return null;
        }

        public BookedServiceInfoModel? UpdateBookingService(BookedServiceInfoModel service)
        {
            var serviceInfo = ToBookedService(service);

            context.BookedServices.Update(serviceInfo);

            return ToBookedServiceModel(serviceInfo);
        }

        public AppointmentInfoModel UpdateBookingInfo(AppointmentInfoModel booking)
        {
            var info = MapBookingModelToBooking(booking);
            context.Appointments.Update(info);

            return MapBookingToBookingModel(info);

        }

        public bool DeleteBookingInfo(Guid bookId)
        {
            var appointment = context.Appointments.Find(bookId);

            if (appointment != null)
            {
                context.Appointments.Remove(appointment);
                return true;
            }

            return false;
        }

        public void DeleteBookingService(Guid appointmentId)
        {
            var service = context.BookedServices.Find(appointmentId);

            if (service != null)
            context.BookedServices.Remove(service);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private Appointment MapBookingModelToBooking(AppointmentInfoModel book)
        {
            return new Appointment()
            {
                Id = book.Id,
                Date = book.AppointmentDate,
                AppointmentType = book.Type ?? "checkup",
                ClinicId = book.ClinicId,
                CustomerId = book.CustomerId,
                DentistId = book.DentistId,
                SlotId = book.ClinicSlotId,
                Status = book.Status,
                CycleCount = book.CyleCount,
                DentistNote = book.Note,
                OriginalAppointment = book.OriginalAppoinment,
                PriceFinal = book.AppointmentFee,
                
            };
        }

        private AppointmentInfoModel MapBookingToBookingModel(Appointment appointment)
        {
            return new AppointmentInfoModel()
            {
                Id = appointment.Id,
                AppointmentDate = appointment.Date,
                Type = appointment.AppointmentType,
                CreationTime = appointment.CreationTime,
                ClinicId = appointment.ClinicId,
                CustomerId = appointment.CustomerId,
                DentistId = appointment.DentistId,
                ClinicSlotId = appointment.SlotId,
                Status = appointment.Status,
                Note = appointment.DentistNote,
                CyleCount = appointment.CycleCount,
                OriginalAppoinment = appointment.OriginalAppointment!,
                AppointmentFee = appointment.PriceFinal,
                
            };
        }

        private BookedServiceInfoModel ToBookedServiceModel(BookedService bookedService)
        {
            return new BookedServiceInfoModel()
            {
                AppointmentId = bookedService.AppointmentId,
                ClinicServiceId = bookedService.ServiceId,
                Price = bookedService.Price,
            };
        }

        private BookedService ToBookedService(BookedServiceInfoModel bookedServiceInfo)
        {
            return new BookedService()
            {
                AppointmentId = bookedServiceInfo.AppointmentId,
                ServiceId = bookedServiceInfo.ClinicServiceId,
                Price = bookedServiceInfo.Price,
            };
        }
    }
}
