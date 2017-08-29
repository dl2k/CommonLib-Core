using System;
using System.ComponentModel;

namespace CommonLib.Validation
{
    /// <summary>
    /// 验证枚举
    /// </summary>
    public enum VerificationType
    {
        /// <summary>
        /// 不能为空
        /// </summary>
        [Description("不能为空")]
        NOT_NULL_OR_EMPTY,

        /// <summary>
        /// 不能为0
        /// </summary>
        [Description("不能为0")]
        NOT_ZERO,

        /// <summary>
        /// 不能为空或为0
        /// </summary>
        [Description("不能为空或为0")]
        NOT_EMPTY_OR_ZERO,

        /// <summary>
        /// 为手机号码格式
        /// </summary>
        [Description("必须是手机号码格式")]
        IS_PHONE,

        /// <summary>
        /// 为手机号码格式
        /// </summary>
        [Description("必须为手机号码格式或者为空")]
        EMPTY_OR_IS_PHONE,

        /// <summary>
        /// 为邮件格式
        /// </summary>
        [Description("必须为邮件格式")]
        IS_EMAIL,

        /// <summary>
        /// 为身份证格式
        /// </summary>
        [Description("必须为身份证格式")]
        IS_ID_CARD,

        /// <summary>
        /// 为正整数
        /// </summary>
        [Description("必须为正整数")]
        IS_UINT,

        /// <summary>
        /// 中文名验证
        /// </summary>
        [Description("必须为中文姓名")]
        IS_CHINESE_NAME,

        /// <summary>
        /// 登录名
        /// </summary>
        [Description("必须为手机号或者客服用户名")]
        IS_LOGIN,

        /// <summary>
        /// 登录名
        /// </summary>
        [Description("必须为客服用户名")]
        IS_USERNAME,

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("必须为登录密码格式(6到16位字符)")]
        IS_LOGIN_PASSWORD,

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("必须为正数，可有小数位")]
        IS_VALID_NUMBER,

        /// <summary>
        /// 中文英文数字集合
        /// </summary>
        [Description("必须为中文英文数字的集合")]
        IS_CHARACTER,

        /// <summary>
        /// ID编号
        /// </summary>
        [Description("必须为ID编号")]
        IS_ID_NO,

        /// <summary>
        /// money值
        /// </summary>
        [Description("必须金额数值")]
        IS_MONEY_AMOUNT,


        /// <summary>
        /// 客服帐号前缀
        /// </summary>
        [Description("前缀格式不正确")]
        IS_KF_PREFIX,



        /// <summary>
        /// 银行卡号
        /// </summary>
        [Description("银行卡号格式不正确")]
        IS_BANK_CARDNO,

    }

    /// <summary>
    /// 验证实体的特性
    /// </summary>
    public class VerificationEntityAttribute : Attribute
    {
        /// <summary>
        /// 验证的属性
        /// </summary>
        public VerificationType Type;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage;
    }

    /// <summary>
    /// 设置默认值的特性(暂时只支持string类型)
    /// </summary>
    public class SetDefaultValueAttribute : Attribute
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;
    }
}
