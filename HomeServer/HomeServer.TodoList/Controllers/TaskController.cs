using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeServer.Database;
using HomeServer.TodoList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeServer.TodoList.Controllers
{
    /// <summary>
    /// Accesses the data nested within the HomeServer task database.
    /// </summary>
    /// <param name="DbContext"></param>
    public class TaskController(
        HomeServerContext DbContext,
        ILogger<TaskController> Logger
    ) : IController
    {
        /// <inheritdoc/>
        public async Task<IActionResult> DeleteTaskByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("DELETE task with ID ({id}) called.", id);

            try
            {
                // Getting the specific task.
                HomeServerTask? task = await DbContext.Tasks.FindAsync([id], cancellationToken);

                if (task is null)
                {
                    return new NotFoundResult();
                }

                // Removing the task.
                DbContext.Tasks.Remove(task);
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException cancelled)
            {
                Logger.LogInformation("DELETE task by ID was cancelled: {message}", cancelled.Message);
            }

            Logger.LogInformation("DELETE completed successfully.");
            return new NoContentResult();
        }

        /// <inheritdoc/>
        public async Task<ActionResult<TaskInfoDto>> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("GET task with ID ({id}) called.", id);
            HomeServerTask? task = null;

            try
            {
                // Getting the specific task.
                task = await DbContext.Tasks.FindAsync([id], cancellationToken);
            }
            catch (OperationCanceledException cancelled)
            {
                Logger.LogInformation("GET task by ID was cancelled: {message}", cancelled.Message);
            }

            if (task is null)
            {
                return new NotFoundResult();
            }

            Logger.LogInformation("GET task by ID completed sucessfully.");
            return new OkObjectResult(task.FromTask());
        }

        /// <inheritdoc/>
        public async Task<ActionResult<ICollection<TaskInfoDto>>> GetTasksAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("GET tasks called.");
            List<HomeServerTask> tasks = [];

            try
            {
                // Getting every task in the database as a list.
                tasks = await DbContext.Tasks.ToListAsync(cancellationToken);
            }
            catch (OperationCanceledException cancelled)
            {
                Logger.LogInformation("GET tasks was cancelled: {message}", cancelled.Message);
            }

            // Returning that data as transfer objects.
            Logger.LogInformation("GET tasks completed sucessfully.");
            return new OkObjectResult(tasks.Select(task => task.FromTask()));
        }

        /// <inheritdoc/>
        public async Task<ActionResult<int>> PostTaskAsync(PostTaskInfoDto? body, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("POST task called.");

            if (body is null)
            {
                Logger.LogInformation("POST task failed: Bad Request.");
                return new BadRequestResult();
            }

            HomeServerTask newTask = body.FromTaskDto();

            try
            {
                // Adding the new task and saving the changes.
                await DbContext.Tasks.AddAsync(newTask, cancellationToken);
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException cancelled)
            {
                Logger.LogInformation("POST task was cancelled: {message}", cancelled.Message);
            }

            Logger.LogInformation("POST task completed sucessfully.");
            return new OkObjectResult(newTask.Id);
        }

        /// <inheritdoc/>
        public async Task<ActionResult<TaskInfoDto>> PutTaskByIdAsync(int id, PutTaskInfoDto? body, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("PUT task called on ID: {id}.", id);

            if (body is null)
            {
                Logger.LogInformation("PUT task by ID failed: Bad Request.");
                return new BadRequestResult();
            }

            try
            {
                HomeServerTask? resp = await DbContext.Tasks.FindAsync([id], cancellationToken);

                if (resp is null)
                {
                    Logger.LogInformation("PUT task by ID failed: Not found.");
                    return new NotFoundResult();
                }

                // Setting the title.
                if (body.Title is not null)
                {
                    resp.Title = body.Title;
                }

                // Setting the description.
                if (body.Description is not null)
                {
                    resp.Description = body.Description;
                }

                // Setting the Due Date.
                if (body.DueDate is not null)
                {
                    resp.DueDate = DateTime.Parse(body.DueDate);
                }

                // Setting the Estimated time to take.
                if (body.EstimatedTime is not null)
                {
                    resp.EstimatedTime = body.EstimatedTime.Value;
                }

                // Setting the Completion state of the Task.
                if (body.IsComplete is not null)
                {
                    resp.IsComplete = body.IsComplete;
                }
            }
            catch (OperationCanceledException cancelled)
            {
                Logger.LogInformation("PUT task by ID was cancelled: {message}", cancelled.Message);
            }
            
            await DbContext.SaveChangesAsync(cancellationToken);            
            Logger.LogInformation("PUT task by ID completed sucessfully.");
            return new OkObjectResult(id);
        }
    }
}

/// <summary>
/// Extension methods used within the HomeServer task controller.
/// </summary>
public static class TaskControllerExtensions
{
    /// <summary>
    /// Converts from a persistent data object to a more permanents data object.
    /// </summary>
    /// <param name="task">The persistent object having data pulled from.</param>
    /// <returns>A transfer object containing persistent data.</returns>
    public static TaskInfoDto FromTask(this HomeServerTask task)
    {
        return new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate.ToString(),
            EstimatedTime = task.EstimatedTime,
            IsComplete = task.IsComplete
        };
    }
    
    /// <summary>
    /// Converts the temporary data to persistent task data.
    /// </summary>
    /// <param name="dto">The transfer object having data pulled from.</param>
    /// <returns>A persistent data model of the original object.</returns>
    public static HomeServerTask FromTaskDto(this PostTaskInfoDto dto)
    {
        return new()
        {
            Title = dto.Title,
            IsComplete = dto.IsComplete,
            Description = dto.Description,
            DueDate = DateTime.Parse(dto.DueDate!),
            EstimatedTime = dto.EstimatedTime!.Value,
            StartTime = DateTime.Now
        };
    }
}