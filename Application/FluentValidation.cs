using Application.DTOs;
using FluentValidation;

namespace Application
{
    public class TaskListCreateDtoValidator : AbstractValidator<CreateDto>
    {
        public TaskListCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 255);
        }
    }

    public class TaskListUpdateDtoValidator : AbstractValidator<UpdateDto>
    {
        public TaskListUpdateDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Task list Id is required.");
            RuleFor(x => x.Name).NotEmpty().Length(1, 255);
        }
    }

    public class TaskListShareDtoValidator : AbstractValidator<ShareDto>
    {
        public TaskListShareDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
