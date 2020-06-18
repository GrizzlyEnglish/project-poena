using System.Collections.Generic;

namespace Project_Poena.Input
{
    /// <summary>
    /// Input Gather interface allows for each sub set to determine how it needs to 
    /// gather input from the user, taking a list of actions and returning the actions it
    /// gathered
    /// </summary>
    public interface IInputGather 
    {
        /// <summary>
        /// Gathers the input
        /// </summary>
        /// <returns>
        /// List of InputAction that were gathered
        /// </returns>
        List<InputAction> Gather();
    }
}