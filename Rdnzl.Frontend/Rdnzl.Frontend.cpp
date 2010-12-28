// This is the main DLL file.

#include "stdafx.h"

#include "Rdnzl.Frontend.h"

using namespace Rdnzl::Backend;

extern "C" {

	// From DelegateAdpater.h
	__declspec(dllexport) void setFunctionPointers(void *callback_fp, void *release_fp)
	{
		API::SetFunctionPointers(IntPtr(callback_fp), IntPtr(release_fp));
	}


	// From DelegateAdapterBuilder.h
	__declspec(dllexport) void *buildDelegateType(const __wchar_t *typeName, void *returnType, void *argTypes)
	{
		return API::buildDelegateType(IntPtr((void*)typeName), returnType, argTypes);
	}

	// From DotNetContainer.h

	__declspec(dllexport) void *makeTypeFromName(const __wchar_t *type)
	{
		return API::makeTypeFromName(IntPtr((void*)type));
	}

	__declspec(dllexport) void *makeTypedNullDotNetContainer(const __wchar_t *type)
	{
		return API::makeTypedNullDotNetContainer(IntPtr((void*)type));
	}

	__declspec(dllexport) void *makeDotNetContainerFromBoolean(bool b)
	{
		return API::makeDotNetContainerFromBoolean(b);
	}

	__declspec(dllexport) void *makeDotNetContainerFromInt(int n)
	{
		return API::makeDotNetContainerFromInt(n);
	}

	__declspec(dllexport) void *makeDotNetContainerFromLong(const __wchar_t *s)
	{
		return API::makeDotNetContainerFromLong(IntPtr((void*)s));
	}

	__declspec(dllexport) void *makeDotNetContainerFromFloat(float d)
	{
		return API::makeDotNetContainerFromFloat(d);
	}

	__declspec(dllexport) void *makeDotNetContainerFromDouble(double d)
	{
		return API::makeDotNetContainerFromDouble(d);
	}

	__declspec(dllexport) void *makeDotNetContainerFromChar(__wchar_t c)
	{
		return API::makeDotNetContainerFromChar(c);
	}

	__declspec(dllexport) void *makeDotNetContainerFromString(const __wchar_t *s)
	{
		return API::makeDotNetContainerFromString(IntPtr((void*)s));
	}

	__declspec(dllexport) bool DotNetContainerIsNull(void *ptr)
	{
		return API::DotNetContainerIsNull(ptr);
	}

	__declspec(dllexport) int getDotNetContainerTypeStringLength(void *ptr)
	{
		return API::getDotNetContainerTypeStringLength(ptr);
	}

	__declspec(dllexport) void getDotNetContainerTypeAsString(void *ptr, __wchar_t *s)
	{
		return API::getDotNetContainerTypeAsString(ptr, IntPtr((void*)s));
	}

	__declspec(dllexport) int getDotNetContainerObjectStringLength(void *ptr)
	{
		return API::getDotNetContainerObjectStringLength(ptr);
	}

	__declspec(dllexport) void getDotNetContainerObjectAsString(void *ptr, __wchar_t *s)
	{
		API::getDotNetContainerObjectAsString(ptr, IntPtr((void*)s));
	}

	__declspec(dllexport) int getDotNetContainerIntValue(void *ptr)
	{
		return API::getDotNetContainerIntValue(ptr);
	}

	__declspec(dllexport) __wchar_t getDotNetContainerCharValue(void *ptr)
	{
		return API::getDotNetContainerCharValue(ptr);
	}

	__declspec(dllexport) bool getDotNetContainerBooleanValue(void *ptr)
	{
		return API::getDotNetContainerBooleanValue(ptr);
	}

	__declspec(dllexport) double getDotNetContainerDoubleValue(void *ptr)
	{
		return API::getDotNetContainerDoubleValue(ptr);
	}

	__declspec(dllexport) float getDotNetContainerSingleValue(void *ptr)
	{
		return API::getDotNetContainerSingleValue(ptr);
	}

	__declspec(dllexport) void refDotNetContainerType(void *ptr)
	{
		API::refDotNetContainerType(ptr);
	}

	__declspec(dllexport) void unrefDotNetContainerType(void *ptr)
	{
		API::unrefDotNetContainerType(ptr);
	}

	__declspec(dllexport) void freeDotNetContainer(void *ptr)
	{
		API::freeDotNetContainer(ptr);
	}

	__declspec(dllexport) void *copyDotNetContainer(void *ptr)
	{
		return API::copyDotNetContainer(ptr);
	}

	// function definition is in InvocationResult.cpp
	__declspec(dllexport) void *setDotNetContainerTypeFromString(const __wchar_t *type, void *ptr)
	{
		return API::setDotNetContainerTypeFromString(IntPtr((void*)type), ptr);
	}

	__declspec(dllexport) void *setDotNetContainerTypeFromContainer(void *type, void *ptr)
	{
		return API::setDotNetContainerTypeFromContainer(type, ptr);
	}


	// From Field.h
	__declspec(dllexport) void *getInstanceFieldValue(const __wchar_t *fieldName, void *target)
	{
		return API::getInstanceFieldValue(IntPtr((void*)fieldName), target);
	}

	__declspec(dllexport) void *getStaticFieldValue(const __wchar_t *fieldName, void *type)
	{
		return API::getStaticFieldValue(IntPtr((void*)fieldName), type);
	}

	__declspec(dllexport) void *setInstanceFieldValue(const __wchar_t *fieldName, void *target, void *newValue)
	{
		return API::setInstanceFieldValue(IntPtr((void*)fieldName), target, newValue);
	}

	__declspec(dllexport) void *setStaticFieldValue(const __wchar_t *fieldName, void *type, void *newValue)
	{
		return API::setStaticFieldValue(IntPtr((void*)fieldName), type, newValue);
	}

	__declspec(dllexport) void *getInstanceFieldValueDirectly(void *fieldInfo, void *target)
	{
		return API::getInstanceFieldValueDirectly(fieldInfo, target);
	}

	__declspec(dllexport) void *getStaticFieldValueDirectly(void *fieldInfo)
	{
		return API::getStaticFieldValueDirectly(fieldInfo);
	}

	__declspec(dllexport) void *setInstanceFieldValueDirectly(void *fieldInfo, void *target, void *newValue)
	{
		return API::setInstanceFieldValueDirectly(fieldInfo, target, newValue);
	}

	__declspec(dllexport) void *setStaticFieldValueDirectly(void *fieldInfo, void *newValue)
	{
		return API::setStaticFieldValueDirectly(fieldInfo, newValue);
	}

	// From InvocationResult.h
	__declspec(dllexport) bool InvocationResultIsVoid(void *ptr)
	{
		return API::InvocationResultIsVoid(ptr);
	}

	__declspec(dllexport) bool InvocationResultIsException(void *ptr)
	{
		return API::InvocationResultIsException(ptr);
	}

	__declspec(dllexport) void *getDotNetContainerFromInvocationResult(void *ptr)
	{
		return API::getDotNetContainerFromInvocationResult(ptr);
	}

	__declspec(dllexport) void freeInvocationResult(void *ptr)
	{
		return API::freeInvocationResult(ptr);
	}
	
	// From InvokeContructor.h
  __declspec(dllexport) void* invokeConstructor(void *type, int nargs, void *args[])
  {
	  return API::invokeConstructor(type, nargs, args);
  }

  // From InvokeMember.h
  __declspec(dllexport) void *invokeInstanceMember(const __wchar_t *methodName, void *target, int nargs, void *args[])
  {
	  return API::invokeInstanceMember(IntPtr((void*)methodName), target, nargs, args);
  }

  __declspec(dllexport) void* invokeInstanceMemberDirectly(void *methodInfo, int nargs, void *args[])
  {
	  return API::invokeInstanceMemberDirectly(methodInfo, nargs, args);
  }

  __declspec(dllexport) void *invokeStaticMember(const __wchar_t *methodName, void *type, int nargs, void *args[])
  {
	  return API::invokeStaticMember(IntPtr((void*)methodName), type, nargs, args);
  }

  __declspec(dllexport) void* invokeStaticMemberDirectly(void *methodInfo, int nargs, void *args[])
  {
	  return API::invokeStaticMemberDirectly(methodInfo, nargs, args);
  }

  __declspec(dllexport) void *getArrayElement(void *ptr, int index)
  {
	  return API::getArrayElement(ptr, index);
  }


  // From Property.h
  __declspec(dllexport) void *getInstancePropertyValue(const __wchar_t *propertyName, void *target, int nargs, void *args[])
  {
	  return API::getInstancePropertyValue(IntPtr((void*)propertyName), target, nargs, args);
  }

  __declspec(dllexport) void *setInstancePropertyValue(const __wchar_t *propertyName, void *target, int nargs, void *args[])
  {
	  return API::setInstancePropertyValue(IntPtr((void*)propertyName), target, nargs, args);
  }

  __declspec(dllexport) void *getStaticPropertyValue(const __wchar_t *propertyName, void *type, int nargs, void *args[])
  {
	  return API::getStaticPropertyValue(IntPtr((void*)propertyName), type, nargs, args);
  }

  __declspec(dllexport) void *setStaticPropertyValue(const __wchar_t *propertyName, void *type, int nargs, void *args[])
  {
	  return API::setStaticPropertyValue(IntPtr((void*)propertyName), type, nargs, args);
  }

  __declspec(dllexport) void *getInstancePropertyValueDirectly(void *propertyInfo, int nargs, void *args[])
  {
	  return API::setInstancePropertyValueDirectly(propertyInfo, nargs, args);
  }

  __declspec(dllexport) void *getStaticPropertyValueDirectly(void *propertyInfo, int nargs, void *args[])
  {
	  return API::getStaticPropertyValueDirectly(propertyInfo, nargs, args);
  }

  __declspec(dllexport) void *setInstancePropertyValueDirectly(void *propertyInfo, int nargs, void *args[])
  {
	  return API::setInstancePropertyValueDirectly(propertyInfo, nargs, args);
  }

  __declspec(dllexport) void *setStaticPropertyValueDirectly(void *propertyInfo, int nargs, void *args[])
  {
	  return API::setStaticPropertyValueDirectly(propertyInfo, nargs, args);
  }

  __declspec(dllexport) void __stdcall DllEnsureInit(void) {
	  System::Threading::Thread::CurrentThread->ApartmentState = System::Threading::ApartmentState::STA;
  }

  __declspec(dllexport) void __stdcall DllForceTerm(void) {
  }

}

