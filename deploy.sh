#!/bin/bash

# ==============================================
#    KMS DEPLOYMENT SCRIPT - V2.1
# ==============================================

# --- CONFIG COLORS ---
NC='\033[0m' 
B_BLACK='\033[1;30m'
RED='\033[0;31m'    B_RED='\033[1;31m'
GREEN='\033[0;32m'  B_GREEN='\033[1;32m'
B_YELLOW='\033[1;33m'
BLUE='\033[0;34m'   B_BLUE='\033[1;34m'
B_MAGENTA='\033[1;35m'
CYAN='\033[0;36m'   B_CYAN='\033[1;36m'
B_WHITE='\033[1;37m'
BG_RED='\033[41m'

# --- LOG FUNCTIONS ---
log_info() { echo -e "  ${B_BLUE}ℹ️   [INFO]${NC} $1"; }
log_process() { echo -e "  ${B_CYAN}⚙️   [RUNNING]${NC} $1..."; }
log_success() { echo -e "  ${B_GREEN}✅  [SUCCESS]${NC} $1"; }
log_error() { 
    echo -e "\n${BG_RED}${B_WHITE} 🛑 CRITICAL ERROR 🛑 ${NC}"
    echo -e "${B_RED}👉 $1${NC}\n"
}

separator() { echo -e "\n${B_BLUE}════════════════${B_MAGENTA}════════════════${B_BLUE}════════════════${NC}"; }

log_step() {
    separator
    echo -e "${B_MAGENTA}🚀  STEP $1:${NC} ${B_WHITE}$2${NC}"
    separator
    echo ""
}

# --- DRAW PROJECT HEADER ---
 
 
 # ---  (FLOWER GARDEN HD) ---
draw_header() {
    clear
    printf "\n"
    # Dòng 1: Đỉnh hoa
    printf "    ${B_RED}     _ _      ${B_YELLOW}    .-.      ${B_MAGENTA}     ,      ${NC}\n"
    printf "    ${B_RED}   _{ ' }_    ${B_YELLOW}   /   \     ${B_MAGENTA}    / \     ${NC}\n"
    
    # Dòng 2: Thân hoa + Tên dự án
    printf "    ${B_RED}  { .'.'. }   ${B_YELLOW}  |  ${B_WHITE}o${B_YELLOW}  |     ${B_MAGENTA}   (   )    ${NC}  ${B_CYAN}KMS{NC}\n"
    printf "    ${B_RED}   {  '  }    ${B_YELLOW}   \   /     ${B_MAGENTA}    \ /     ${NC}  ${B_GREEN}Nature Deploy${NC}\n"
    
    # Dòng 3: Đài hoa và Cành lá
    printf "    ${B_RED}    \___/     ${B_YELLOW}    '-'      ${B_MAGENTA}     |      ${NC}\n"
    printf "    ${B_GREEN}      |       ${B_GREEN}     |       ${B_GREEN}     |      ${NC}\n"
    printf "    ${B_GREEN}    (\|/)     ${B_GREEN}   --|--     ${B_GREEN}   --|--    ${NC}\n"
    printf "    ${B_GREEN}      |       ${B_GREEN}     |       ${B_GREEN}     |      ${NC}\n"
    printf "    ${B_GREEN}      |       ${B_GREEN}    / \      ${B_GREEN}    /|\     ${NC}\n"
    
    # Dòng 4: Chậu hoa
    printf "   ${B_WHITE} [===================================] ${NC}\n"
    printf "   ${B_WHITE} \___________________________________/ ${NC}\n"
    printf "\n"
    echo -e "${B_MAGENTA}    🌸 Server đang nở hoa... À nhầm, đang khởi động! 🌸${NC}\n"
    sleep 1
}

# ==============================================
#    RUN THE DEPLOYMENT PROCESS
# ==============================================

draw_header

# --- STEP 1: GIT PULL ---
log_step "1" "Syncing with the latest source code (Git)"
log_process "Running: git pull"

if git_output=$(git pull 2>&1); then
    echo -e "${CYAN}${git_output}${NC}"
    log_success "Source code is up to date!"
else
    echo -e "${RED}${git_output}${NC}"
    log_error "Git pull failed! Check for any code conflicts."
    exit 1
fi

# --- STEP 2: DOCKER COMPOSE DOWN ---
log_step "2" "Cleaning up old containers"
log_process "Running: docker compose down"

if docker compose down; then
    log_success "Old containers have been stopped and removed."
else
    log_error "Failed to stop the containers."
    exit 1
fi

# --- STEP 3: DOCKER COMPOSE UP ---
log_step "3" "Building and launching FGInventory"
log_process "Running: docker compose up --build -d"

if docker compose up --build -d --remove-orphans; then 
    log_success "System launched successfully!"
else
    log_error "Error during Docker build."
    exit 1
fi

# --- FINAL STEP ---
separator
echo -e "\n${B_GREEN}🎉✨  DEPLOYMENT SUCCESSFUL! APP IS ONLINE.  ✨🎉${NC}\n"
separator

log_info "Current container status:"
sleep 1
docker compose ps
echo ""
