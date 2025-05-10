using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Core.Exceptions;
using LangApp.Core.ValueObjects.Assignments;

namespace LangApp.Application.Assignments.Extensions;

public static class ActivityDetailsExtensions
{
    public static ActivityDetails ToValueObject(this ActivityDetailsDto detailsDto)
    {
        return detailsDto switch
        {
            FillInTheBlankActivityDetailsDto fillInTheBlankDto =>
                fillInTheBlankDto.ToValueObject(),

            PronunciationActivityDetailsDto pronunciationDto =>
                pronunciationDto.ToValueObject(),

            MultipleChoiceActivityDetailsDto multipleChoiceDto =>
                multipleChoiceDto.ToValueObject(),

            _ => throw new LangAppException($"Unsupported activity type: {detailsDto.GetType().Name}")
        };
    }
}
