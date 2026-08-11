using System.ComponentModel.DataAnnotations;

namespace MyWeb.Portal.Apps;

public sealed class AppDefinitionInput
{
    [Required, StringLength(80)]
    [Display(Name = "서비스 이름")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "영문 소문자, 숫자, 하이픈만 사용할 수 있습니다.")]
    [Display(Name = "URL 식별자")]
    public string Slug { get; set; } = string.Empty;

    [Required, StringLength(500)]
    [Display(Name = "내부 URL")]
    public string InternalUrl { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Display(Name = "분류")]
    public string Category { get; set; } = "기타";

    [StringLength(80)]
    [Display(Name = "아이콘")]
    public string Icon { get; set; } = "app";

    [Required, StringLength(200)]
    [RegularExpression("^/.*", ErrorMessage = "/로 시작해야 합니다.")]
    [Display(Name = "상태 확인 경로")]
    public string HealthPath { get; set; } = "/";

    [Display(Name = "사용")]
    public bool Enabled { get; set; } = true;

    [Range(-10000, 10000)]
    [Display(Name = "메뉴 순서")]
    public int SortOrder { get; set; }
}
