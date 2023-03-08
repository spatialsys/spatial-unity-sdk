namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialValidationSummary
    {
        public enum Result
        {
            Failed,
            PassedWithWarnings,
            PassedWithNoWarnings
        }

        public SpatialTestResponse[] warnings;
        public SpatialTestResponse[] errors;

        public Result result
        {
            get
            {
                if (errors?.Length > 0)
                {
                    return SpatialValidationSummary.Result.Failed;
                }
                else if (warnings?.Length > 0)
                {
                    return SpatialValidationSummary.Result.PassedWithWarnings;
                }

                return SpatialValidationSummary.Result.PassedWithNoWarnings;
            }
        }

        public bool failed => result == Result.Failed;
        public bool passedWithWarnings => result == Result.PassedWithWarnings;
        public bool passedWithNoWarnings => result == Result.PassedWithNoWarnings;
        public bool passed => passedWithNoWarnings || passedWithWarnings;
    }
}
