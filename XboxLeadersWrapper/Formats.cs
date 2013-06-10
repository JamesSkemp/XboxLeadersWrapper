namespace XboxLeadersWrapper
{
	/// <summary>
	/// Format to request result in.
	/// </summary>
	public enum ResultFormat
	{
		/// <summary>
		/// Request JSON data.
		/// </summary>
		Json,
		/// <summary>
		/// Request XML data.
		/// </summary>
		Xml,
		/// <summary>
		/// Request PHP data. Client will need to convert string to PHP.
		/// </summary>
		Php
	}
}
