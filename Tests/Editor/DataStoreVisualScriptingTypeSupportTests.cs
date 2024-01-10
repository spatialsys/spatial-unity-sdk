using System;
using System.Collections.Generic;
using SpatialSys.UnitySDK.VisualScripting;
using UnityEngine;
using NUnit.Framework;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.Tests
{
    [TestFixture]
    public class DataStoreVisualScriptingTypeSupportTests
    {
        [Test]
        public void Test_FromRawToVS_FullRange()
        {
            // Test all supported types
            DataStoreResponseCode responseCode = DataStoreVisualScriptingTypeSupport.TryConvertFromInternalTypes(
                new Dictionary<string, object> {
                    { "null", null },
                    { "string", "string" },
                    { "bool", true },
                    { "int", 1 },
                    { "float", 1.5f },
                    { "double", 223.55 },
                    { "long", 7643650907989348765L },
                    { "decimal", new Decimal(876345876L) },
                    { "vec2", new Vector2(1, 2) },
                    { "vec3", new Vector3(1, 2, 3) },
                    { "vec4", new Vector4(1, 2, 3, 4) },
                    { "quat", Quaternion.Euler(44, 0, 12) },
                    { "color", new Color(0.3f, 0.2f, 0.77f) },
                    { "dateTime", new DateTime(2023, 3, 23, 11, 44, 44) },
                    { "stringArray", new string[] { "test1", null, "test2", "test3" } },
                    { "boolArray", new bool[] { true, false, true } },
                    { "intArray", new int[] { 1, 2, 3 } },
                    { "floatArray", new float[] { 1.5f, 2.5f, 3.5f } },
                    { "dict", new Dictionary<string, object> {
                        { "nestedNull", null },
                        { "nestedString", "string" },
                        { "nestedBool", true },
                        { "nestedInt", 1 },
                        { "nestedFloat", 1.5f },
                        { "nestedDouble", -23.55 },
                        { "nestedLong", -786478648764L },
                        { "nestedDecimal", new Decimal(87668683523L) },
                        { "nestedVec2", new Vector2(1, 2) },
                        { "nestedVec3", new Vector3(1, 2, 3) },
                        { "nestedVec4", new Vector4(1, 2, 3, 4) },
                        { "nestedQuat", Quaternion.Euler(44, 555, 12) },
                        { "nestedColor", Color.clear },
                        { "nestedDateTime", new DateTime(2017, 3, 23, 11, 44, 22) },
                        { "nestedIntArray", new int[] { 1, 2, 3 } },
                        { "nestedBoolArray", new bool[] { true, false, true } },
                        { "nestedFloatArray", new float[] { 1.5f, 2.5f, 3.5f } },
                        { "nestedStringArray", new string[] { "test1", "test2", "test3", null } },
                        { "doubleNestedDictionary", new Dictionary<string, object> {
                            { "nested_int", 55 },
                            { "nested_float_array", new float[] { 1.5f, 2.5f, 3.5f } },
                        }}
                    }},
                    { "emptyDict", new Dictionary<string, object> { } },
                },
                out object convertedValue
            );

            Assert.AreEqual(DataStoreResponseCode.Ok, responseCode);
        }

        [Test]
        public void Test_FromVSToRaw_Primitives()
        {
            object retrievedValue = ConvertToInternalTypes("hi");
            Assert.AreEqual("hi", retrievedValue);
        }

        [Test]
        public void Test_FromVSToRaw_AotList_SupportedTypes()
        {
            AotList intList = new() { 44, 77, 88 };
            object convertedValue = ConvertToInternalTypes(intList);
            Assert.AreEqual(typeof(int[]), convertedValue.GetType());
            Assert.AreEqual(intList.Count, (convertedValue as int[]).Length);
        }

        [Test]
        public void Test_FromVSToRaw_AotList_Empty()
        {
            AotList emptyList = new();
            object convertedValue = ConvertToInternalTypes(emptyList);
            Assert.AreEqual(typeof(string[]), convertedValue.GetType());
            Assert.AreEqual(emptyList.Count, (convertedValue as string[]).Length);
        }

        [Test]
        public void Test_FromVSToRaw_AotList_StringArrayWithNulls()
        {
            AotList list = new() { null, "hey", "howdy", null };
            object convertedValue = ConvertToInternalTypes(list);
            Assert.AreEqual(typeof(string[]), convertedValue.GetType());
            Assert.AreEqual(list.Count, (convertedValue as string[]).Length);
        }

        [Test]
        public void Test_FromVSToRaw_AotList_NullArray() // treated as string[] with null values
        {
            AotList list = new() { null, null, null };
            object convertedValue = ConvertToInternalTypes(list);
            Assert.AreEqual(typeof(string[]), convertedValue.GetType());
            Assert.AreEqual(list.Count, (convertedValue as string[]).Length);
        }

        [Test]
        public void Test_FromVSToRaw_AotList_Exceptions_UnsupportedElementType()
        {
            AotList list = new() {
                new Vector2(1, 2),
                new Vector2(1, 2)
            };
            AssertThrows(DataStoreResponseCode.UnsupportedValueType, () => {
                ConvertToInternalTypes(list);
            });
        }

        [Test]
        public void Test_FromVSToRaw_AotList_Exceptions_NonUniformTypes()
        {
            AotList list = new() { 77, "hey", new Vector2(1, 2) };
            Assert.Throws<System.InvalidCastException>(() => {
                ConvertToInternalTypes(list);
            });
        }

        [Test]
        public void Test_FromVSToRaw_AotDictionary_SupportedTypes()
        {
            AotDictionary dict = new() {
                { "score", 11 },
                { "speed", 54.847f },
                { "lastPosition", Vector3.one }
            };
            object convertedValue = ConvertToInternalTypes(dict);
            Assert.AreEqual(typeof(Dictionary<string, object>), convertedValue.GetType());
            Assert.AreEqual(dict.Count, (convertedValue as Dictionary<string, object>).Count);
        }

        [Test]
        public void Test_FromVSToRaw_AotDictionary_Empty()
        {
            AotDictionary dict = new();
            object convertedValue = ConvertToInternalTypes(dict);
            Assert.AreEqual(typeof(Dictionary<string, object>), convertedValue.GetType());
            Assert.AreEqual(dict.Count, (convertedValue as Dictionary<string, object>).Count);
        }

        [Test]
        public void Test_FromVSToRaw_AotDictionary_Exceptions_UnsupportedKeyType()
        {
            AotDictionary dict = new AotDictionary {
                { 44, "hi" }
            };
            AssertThrows(DataStoreResponseCode.UnsupportedDictionaryKeyType, () => {
                ConvertToInternalTypes(dict);
            });
        }

        private object ConvertToInternalTypes(object value)
        {
            DataStoreResponseCode responseCode = DataStoreVisualScriptingTypeSupport.TryConvertToInternalTypes(value, out object convertedValue);
            if (responseCode != DataStoreResponseCode.Ok)
            {
                Exception ex = new("DataStoreVisualScriptingTypeSupportTests: ConvertToInternalTypes failed");
                ex.Data.Add("responseCode", responseCode);
                throw ex;
            }
            return convertedValue;
        }

        public static void AssertThrows(DataStoreResponseCode expectedCode, TestDelegate @delegate)
        {
            Exception ex = Assert.Throws<Exception>(@delegate);
            Assert.AreEqual(expectedCode, ex.Data["responseCode"]);
        }
    }
}
