#!/bin/bash

# Script để chạy tests với format đẹp hơn
# Usage: ./run-tests.sh [additional dotnet test arguments]
# Example: ./run-tests.sh --collect:"XPlat Code Coverage"

echo "Running tests..."
echo "=================="
echo ""

# Chạy tests và capture output
TEST_OUTPUT=$(dotnet test --logger "console;verbosity=normal" "$@" 2>&1)
EXIT_CODE=$?

# Extract test results và format lại
TEST_COUNT=0
PASSED_COUNT=0
FAILED_COUNT=0

while IFS= read -r line; do
    # Kiểm tra nếu dòng chứa "Passed" hoặc "Failed"
    if [[ $line =~ ^[[:space:]]*(Passed|Failed)[[:space:]]+([^[]+) ]]; then
        TEST_COUNT=$((TEST_COUNT + 1))
        STATUS="${BASH_REMATCH[1]}"
        FULL_TEST_NAME="${BASH_REMATCH[2]}"
        
        # Extract time nếu có
        if [[ $line =~ \[([0-9]+\sms)\] ]]; then
            TIME="${BASH_REMATCH[1]}"
        else
            TIME=""
        fi
        
        # Rút gọn tên test (chỉ lấy phần cuối sau dấu chấm cuối cùng)
        TEST_NAME=$(echo "$FULL_TEST_NAME" | sed 's/.*\.\([^.]*\)$/\1/')
        
        # Format output với màu sắc
        if [[ "$STATUS" == "Passed" ]]; then
            PASSED_COUNT=$((PASSED_COUNT + 1))
            printf "\033[0;32m✓\033[0m Test-case %2d: \033[0;32mPASSED\033[0m - %s %s\n" "$TEST_COUNT" "$TEST_NAME" "$TIME"
        else
            FAILED_COUNT=$((FAILED_COUNT + 1))
            printf "\033[0;31m✗\033[0m Test-case %2d: \033[0;31mFAILED\033[0m - %s %s\n" "$TEST_COUNT" "$TEST_NAME" "$TIME"
        fi
    fi
done <<< "$TEST_OUTPUT"

# Hiển thị summary
echo ""
echo "=================="
if [ $FAILED_COUNT -eq 0 ]; then
    echo -e "\033[0;32m✓ All tests passed!\033[0m"
else
    echo -e "\033[0;31m✗ Some tests failed!\033[0m"
fi
echo "Summary:"
echo "  Total tests: $TEST_COUNT"
echo "  Passed: $PASSED_COUNT"
if [ $FAILED_COUNT -gt 0 ]; then
    echo -e "  \033[0;31mFailed: $FAILED_COUNT\033[0m"
fi

# Hiển thị full output nếu có lỗi hoặc nếu muốn debug
if [ $FAILED_COUNT -gt 0 ]; then
    echo ""
    echo "Full output:"
    echo "$TEST_OUTPUT"
fi

# Return exit code dựa trên kết quả
if [ $FAILED_COUNT -gt 0 ] || [ $EXIT_CODE -ne 0 ]; then
    exit 1
else
    exit 0
fi

