﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSecure.Helpers;
using WebSecure.Repository;

namespace WebSecure.Controllers
{
    public class VerifyResetPasswordController : Controller
    {
        private readonly IVerificationRepository _verificationRepository;
        public VerifyResetPasswordController(IVerificationRepository verificationRepository)
        {
            _verificationRepository = verificationRepository;
        }


        [HttpGet]
        public IActionResult Verify(string key, string hashtoken)
        {
            try
            {
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(hashtoken))
                {
                    var arrayVakue = SecurityManager.SplitToken(key);
                    if (arrayVakue != null)
                    {
                        // arrayVakue[1] "UserId"
                        var rvModel = _verificationRepository.GetResetGeneratedToken(arrayVakue[1]);
                        if (rvModel != null)
                        {
                            var result = SecurityManager.IsTokenValid(arrayVakue, hashtoken, rvModel.GeneratedToken);

                            if (result == 1)
                            {
                                TempData["TokenMessage"] = "Sorry Verification Link Expired Please request a new Verification link!";
                                return RedirectToAction("Login", "Portal");
                            }

                            if (result == 2)
                            {
                                TempData["TokenMessage"] = "Sorry Verification Link Expired Please request a new Verification link!";
                                return RedirectToAction("Login", "Portal");
                            }

                            if (result == 0)
                            {
                                HttpContext.Session.SetString("VerificationUserId", arrayVakue[1]);
                                HttpContext.Session.SetString("ActiveVerification", "1");
                                return RedirectToAction("Reset", "ResetPassword");
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
                return RedirectToAction("Login", "Portal");
            }

            TempData["TokenMessage"] = "Sorry Verification Failed Please request a new Verification link!";
            return RedirectToAction("Login", "Portal");
        }
    }
}