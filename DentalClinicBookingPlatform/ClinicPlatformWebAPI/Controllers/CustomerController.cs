﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicPlatformServices.Contracts;
using ClinicPlatformServices;
using ClinicPlatformDTOs.UserModels;
using ClinicPlatformWebAPI.Helpers.Models;
using Microsoft.AspNetCore.Authorization;
using ClinicPlatformObjects.MiscModels;
using ClinicPlatformWebAPI.Helpers.ModelMapper;
using ClinicPlatformObjects.UserModels.CustomerModel;

namespace ClinicPlatformWebAPI.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IUserService userService;

        public CustomerController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize(Roles="Customer")]
        public ActionResult<IHttpResponseModel<CustomerInfoViewModel>> GetCustomerInformationint()
        {
            UserInfoModel customer = (UserInfoModel?) HttpContext.Items["user"]!;

            if (customer == null)
            {
                return BadRequest(new HttpResponseModel()
                {
                    StatusCode = 400,
                    Message = "User not found",
                });

            }

            return Ok(new HttpResponseModel()
            {
                StatusCode = 200,
                Message = "Success",
                Content = UserInfoMapper.ToCustomerView(customer)
            });
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult<HttpResponseModel> RegisterCustomer([FromBody] CustomerRegistrationModel userInfo)
        {
            UserInfoModel? user = userService.RegisterAccount(UserInfoMapper.FromRegistration(userInfo), "Customer", out var message);
            if (user == null)
            {
                return BadRequest(new HttpResponseModel()
                {
                    StatusCode = 400,
                    Message = "Failed",
                    Detail = message,
                });
            }

            return Ok(new HttpResponseModel()
            {
                StatusCode = 200,
                Message = "OK",
                Detail = "User created successfully!",
            });
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public ActionResult<IHttpResponseModel<CustomerInfoViewModel>> UpdateCustomerIndo([FromBody] CustomerUpdateModel updatedInfo)
        {
            UserInfoModel? customer = (UserInfoModel?)HttpContext.Items["user"]!;

            if (customer.Id != updatedInfo.Id)
            {
                return BadRequest(new HttpResponseModel
                {
                    StatusCode = 400,
                    Message = $"Update failed",
                    Detail = $"User Id and update info Id mismatch! UserId: {customer.Id}, TargetUserId: {updatedInfo.Id}."
                });
            }

            customer.Insurance = updatedInfo.Insurance ?? customer.Insurance;
            customer.Email = updatedInfo.Email ?? customer.Email;
            customer.Fullname = updatedInfo.Fullname ?? customer.Fullname;
            customer.Phone = updatedInfo.Phone ?? customer.Phone;
            customer.Birthdate = updatedInfo.Birthdate ?? customer.Birthdate;
            customer.Sex = updatedInfo.Sex ?? customer.Sex;

            customer = userService.UpdateUserInformation(customer, out var message);

            if (customer == null)
            {
                return BadRequest(new HttpResponseModel
                {
                    StatusCode = 400,
                    Message = $"Update failed",
                    Detail = message
                });
            }
            else
            {
                return Ok(new HttpResponseModel
                {
                    StatusCode = 200,
                    Message = $"Updated successfully.",
                    Detail = $"Update information for customer UserId {customer.Id}",
                    Content = customer
                });
            }
        }

        [HttpPut("activate")]
        [AllowAnonymous]
        public ActionResult<HttpResponseModel> ActivateUserAccount([FromQuery] int userId, [FromQuery] string token)
        {

            if (!userService.ActivateUser(userId, out var message))
            {
                return BadRequest(new HttpResponseModel()
                {
                    StatusCode = 400,
                    Message = "Activition failed",
                    Detail = message,
                });
            }

            return Ok(new HttpResponseModel()
            {
                StatusCode = 200,
                Message = "Activation success",
                Detail = $"Activated user account for user {userId}",
            });
        }

        [HttpPut("deactivate")]
        [Authorize(Roles = "Customer")]
        public ActionResult<HttpResponseModel> InactivateUserAccount()
        {
            UserInfoModel user = (UserInfoModel)HttpContext.Items["user"]!;

            if (!userService.InactivateUser(user.Id, out var message))
            {
                return BadRequest(new HttpResponseModel()
                {
                    StatusCode = 400,
                    Message = "Deactivation failed",
                    Detail = message,
                });
            }

            return Ok(new HttpResponseModel()
            {
                StatusCode = 200,
                Message = "Deactivation success",
            });
        }

        [HttpDelete]
        [Authorize(Roles = "Customer")]
        public ActionResult<HttpResponseModel> DeleteUser()
        {
            UserInfoModel user = (UserInfoModel) HttpContext.Items["user"]!; 

            if (!userService.DeleteUser(user.Id, out var message))
            {
                return BadRequest(new HttpResponseModel()
                {
                    StatusCode = 400,
                    Message = "Delete user failed",
                    Detail = message,
                });
            }


            return Ok(new HttpResponseModel()
            {
                StatusCode = 200,
                Message = "Delete user info success",
            });
        }
    }
}
