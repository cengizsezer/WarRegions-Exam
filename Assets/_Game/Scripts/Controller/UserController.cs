using MyProject.Core.Controllers;
using MyProject.Core.Models;
using MyProject.Core.Services;
using MyProject.Core.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MyProject.Core.Controllers
{
    public class UserController
    {
        #region Injection

      
        private readonly TaskService _taskService;
        public UserController
        (
           
            TaskService taskService)
        {
           
            _taskService = taskService;
        }

        #endregion

        public void Init()
        {
          
        }

        public void LevelWin()
        {
            var levelupTask = new LevelUpGameTask();
            levelupTask.Init();
            _taskService.AddTask(levelupTask);
        }

        public void LevelFail()
        {
            var levelFailGameTask = new LevelFailGameTask();
            levelFailGameTask.Init();
            _taskService.AddTask(levelFailGameTask);
        }
    }
}

