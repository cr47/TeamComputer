﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodProject.Models;
using FoodProject.Abstract;
using System.Text.RegularExpressions;


namespace FoodProject.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository userRepository;
        private UserLogin temp;

        //userRepository gets initialized with the parameter userRepository through constructor depenency injection
        //infrastructure/NinjectDependecyResolver Handles that
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        // GET: User
        //takes you to the login screen
        [HttpGet]
        public ActionResult Index(User user)
        {
            //the user parameter is given to the method through model binding 
            //go to infrastructure/binders/UserModelBinder to see how it is retrieved

            //if the userId is not null someone is signed in
            if (user.UserID != null)
            {
                return RedirectToAction("Index","Home");
            }
            //they aren't signed in got to login screen
            //pass it a UserLogin object to store the username and password the user types in
            return View(temp);
        }
        //the post method from the login screen handles getting user from database and finializing login
        [HttpPost]
        public RedirectToRouteResult Index(User user,UserLogin temp)
        {
            User tempUser;
            String regexString = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            Regex r = new Regex(regexString, RegexOptions.IgnoreCase);
            if(temp.un != null)
            {
                Match m = r.Match(temp.un);
                if (!m.Success || (temp.un == null && temp.pword == null))
                {
                    return RedirectToAction("Index", "User");
                }
            }
            //sets the user object password&name in the session to the UserLogin object temp's password&name
            user.Name = temp.un;
            user.Password = temp.pword;
            //if name is not null and password is  not null, we can search for a user in the database with the same user name
            if (user.Name != null && user.Password != null)
            {
                //selects from database user object that has same name as user name
                //returns object if found, if not found returns null
                tempUser = userRepository.Users.Select(x => x).Where(x => x.Name == user.Name).FirstOrDefault();
                //if null user is not found in database 
                if (tempUser != null)
                {
                    //we found a user with same name now you have to compare passwords
                    //if they match user hase the right sign in 
                    if (tempUser.Password.Equals(user.Password))
                    {
                        //set the user object attributes in the session to the properties from the user object from the database
                        user.UserID = tempUser.UserID;
                        user.Pantrys = tempUser.Pantrys;
                    }
                    //password doesn't match return to sign in
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
            }
            //if userID is not null they were able to sign in so redirect them to home away from sign in page
            if (user.UserID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //there is a username error go back to login
                return RedirectToAction("Index", "User");
            }

        }
        //create user method
        [HttpGet]
        public ViewResult CreateUser()
        {
            //got to create user view
            // give it a UserLogin object to store username and passwords
            return View(temp);
        }
        //method handles data the user submits for signing up
        [HttpPost]
        public RedirectToRouteResult Createuser(User user,UserLogin temp)
        {
            //user is given to use by model binding, temp is passes to us when the user does the post action

            //if the username and password and password 2 are not null start process of creating user
            if(temp.un!=null && temp.pword != null && temp.pword2!=null)
            {
                
                String regexString = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
                Regex r = new Regex(regexString, RegexOptions.IgnoreCase);
                if (temp.un != null)
                {
                    Match m = r.Match(temp.un);
                    if (!m.Success)
                    {
                        return RedirectToAction("CreateUser", "User");
                    }
                }
                //check if passwords are equal
                if (temp.pword.Equals(temp.pword2))
                {
                    //check database to see if user name is taken 
                    User tempUser = userRepository.Users.Select(x => x).Where(x => x.Name == temp.un).FirstOrDefault();

                    //if tempUser == null then the username doesn't exist
                    if (tempUser == null)
                    {
                        //set the user object  properties in session to the USerLogin object properties
                        user.Name = temp.un;
                        user.Password = temp.pword;
                        //adds the user object to the database
                        userRepository.Add(user);
                        //they created a user take them away from the sign up screen
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            //password not matching or non unique user name
            return RedirectToAction("CreateUser", "User");
        }
        //Handles loging out
        public RedirectToRouteResult LogOut(User user)
        {
            //set the user objects properties in the session to null
            user.Name = null;
            user.Pantrys = null;
            user.UserID = null;
            user.Password = null;
            //return to home screen
            return RedirectToAction("Index", "Home");
        }
    }
}