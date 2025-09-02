using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeServer.WebUi
{
    public record TaskCreationDialogResult(
        string Title,
        string Description,
        float EstimatedTime,
        DateTime DueDate
    );
}