using System.Collections.Generic;

namespace EmploImportExample
{
    public class ImportUsersRequestModel
    {
        public ImportUsersRequestModel(string importMode, string requireRegistrationForNewEmployees)
        {
            Mode = importMode;
            RequireRegistrationForNewEmployees = requireRegistrationForNewEmployees;
            Rows = new List<UserDataRow>();
        }

        public string ImportId { get; set; }

        /// <summary>
        /// Data to import
        /// </summary>
        public List<UserDataRow> Rows { get; set; }

        /// <summary>
        /// Indicates if data should be created, updated or both
        /// </summary>
        public string Mode { get; set; }

        public string RequireRegistrationForNewEmployees { get; set; }
    }

    public class UserDataRow : Dictionary<string, string>
    {
    }

    public class ImportUsersResponseModel
    {
        public ImportStatusCode ImportStatusCode { get; set; }
        public string ImportId { get; set; }
        public List<ImportValidationSummaryRow> OperationResults { get; set; }
    }

    public enum ImportStatusCode
    {
        Ok,
        WrongImportId,
        ImportIsFinished
    }

    public class ImportValidationSummaryRow
    {
        public ImportStatuses StatusCode { get; set; }
        public int? EmployeeId { get; set; }
        public string Employee { get; set; }
        public List<string> ErrorColumns { get; set; }
        public List<string> ChangedColumns { get; set; }
        public bool Created { get; set; }
        public string Message { get; set; }
    }

    public enum ImportStatuses
    {
        Ok,
        MissingData,
        InvalidData,
        NotImplemented,
        ObjectAlreadyExists,
        Error,
        Skipped
    }

    public class FinishImportResponseModel
    {
        public ImportStatusCode ImportStatusCode { get; set; }
        public List<int> BlockedUserIds { get; set; }
        public List<UpdateUnitResult> UpdateUnitResults { get; set; }
    }

    public class UpdateUnitResult
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public int? UpdatedUnitId { get; set; }
        public int? OldParentId { get; set; }
        public int? NewParentId { get; set; }
    }

    public class FinishImportRequestModel
    {
        public FinishImportRequestModel(string blockSkippedUsers)
        {
            BlockSkippedUsers = blockSkippedUsers;
        }

        /// <summary>
        /// Id of the import which is being finished
        /// </summary>
        public string ImportId { get; set; }

        /// <summary>
        /// If set to true, all the users which were not present in the given import will be blocked
        /// </summary>
        public string BlockSkippedUsers { get; set; }
    }
}
