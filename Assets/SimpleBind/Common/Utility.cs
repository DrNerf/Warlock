// Copyright (c) 2015, SoftSource Consulting, Inc.
// 
// All rights reserved.
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://unity3d.com/legal/as_terms

using System;
using System.Linq.Expressions;

namespace SimpleBind.Common
{
	public class Utility
	{
		public static string GetPropertyName<T>(Expression<Func<T, object>> selector)
		{
			var expression = selector.Body as MemberExpression;

			if (expression == null && selector.Body is UnaryExpression)
				expression = ((UnaryExpression)selector.Body).Operand as MemberExpression;

			return expression != null ? expression.Member.Name : string.Empty;
		}
	}
}