# GitHub Push Script for Gaokao Game
# 运行此脚本将项目推送到GitHub

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  高考人生模拟器 - GitHub 推送脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查git状态
Write-Host "[1/4] 检查Git状态..." -ForegroundColor Yellow
git status

# 添加所有更改
Write-Host ""
Write-Host "[2/4] 添加所有文件到暂存区..." -ForegroundColor Yellow
git add -A

# 检查是否有更改
$status = git status --porcelain
if ($status) {
    Write-Host "发现更改，准备提交..." -ForegroundColor Green
} else {
    Write-Host "没有发现新更改" -ForegroundColor Gray
}

# 提交更改
Write-Host ""
Write-Host "[3/4] 提交更改..." -ForegroundColor Yellow
$commitMsg = "feat: $(Get-Date -Format 'yyyy-MM-dd') - 项目更新"
git commit -m $commitMsg

# 推送到GitHub
Write-Host ""
Write-Host "[4/4] 推送到GitHub..." -ForegroundColor Yellow
Write-Host "请确保您已配置GitHub认证凭据" -ForegroundColor Yellow
Write-Host ""

git push -u origin main

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  推送成功！🎉" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "您的项目已成功推送到:" -ForegroundColor Cyan
    Write-Host "  https://github.com/fbi02042026/gaokao_game" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  推送失败 😢" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "可能的原因:" -ForegroundColor Yellow
    Write-Host "  1. 网络连接问题" -ForegroundColor White
    Write-Host "  2. GitHub认证失败" -ForegroundColor White
    Write-Host "  3. 远程仓库不存在" -ForegroundColor White
    Write-Host ""
    Write-Host "解决方案:" -ForegroundColor Yellow
    Write-Host "  1. 检查网络连接" -ForegroundColor White
    Write-Host "  2. 运行: git remote set-url origin https://你的token@github.com/fbi02042026/gaokao_game.git" -ForegroundColor White
    Write-Host "  3. 在GitHub上创建仓库: https://github.com/new" -ForegroundColor White
    Write-Host ""
}

Write-Host "按任意键退出..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
