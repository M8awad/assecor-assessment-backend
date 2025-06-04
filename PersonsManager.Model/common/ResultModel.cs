using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Model.common
{
    public class ResultModel
    {
        public string? ModelStateError { get; set; }
        public object? Data { get; set; }
        public bool Saved { get; set; }
        public string? ID { get; set; }
        public string? ServerMessage { get; set; } = "Operation completed successfully";
    }

    /// <summary>
    /// Generic version of ResultModel for strongly typed data
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ResultModel<T> : ResultModel
    {
        public new T? Data { get; set; }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static ResultModel<T> Success(T data, string message = "Operation completed successfully", string id = null)
        {
            return new ResultModel<T>
            {
                Data = data,
                Saved = true,
                ServerMessage = message,
                ID = id,
                ModelStateError = null
            };
        }

        /// <summary>
        /// Creates a failed result
        /// </summary>
        public static ResultModel<T> Failure(string errorMessage, string modelStateError = null)
        {
            return new ResultModel<T>
            {
                Data = default(T),
                Saved = false,
                ServerMessage = errorMessage,
                ModelStateError = modelStateError,
                ID = null
            };
        }
    }

    /// <summary>
    /// Base view model for get operations that includes additional properties
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class BasePersonView<T>
    {
        public string? ServerMessage { get; set; }
        public T? Data { get; set; }
        public T? UData { get; set; } // For update data if needed
    }
}
