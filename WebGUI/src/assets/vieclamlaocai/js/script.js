$(document).ready(function () {
  initLibs()
  initDomEvent()
  initSlide()
});

function initLibs() {
  $(".fancybox").fancybox({
    //....
  });
  AOS.init();
}

function initSlide() {
  $('.js-list-group-slide').length && $('.js-list-group-slide').owlCarousel({
    items: 1,
    slideSpeed: 1000,
    autoplay: true,
    loop: true,
    smartSpeed: 1000,
    margin: 12,
    fluidSpeed: 500,
    autoplayTimeout: 5000,
    dots: true,
    nav: true,
    animateIn: 'fadeIn',
    animateOut: 'fadeOut',
    navText: [
      '<img class="slide-nav" src="./img/icon/icon-arrow-left-circle.svg" alt="">',
      '<img class="slide-nav" src="./img/icon/icon-arrow-right-circle.svg" alt="">',
    ],
    onInitialized: function (event) {
      updatePagination(event, '.js-list-group-slide');
    },
    onChanged: function (event) {
      updatePagination(event, '.js-list-group-slide');
    }
  });

  $('.js-list-group2-slide').length && $('.js-list-group2-slide').owlCarousel({
    items: 1,
    slideSpeed: 1000,
    autoplay: true,
    loop: true,
    smartSpeed: 1000,
    margin: 12,
    fluidSpeed: 500,
    autoplayTimeout: 5000,
    dots: true,
    nav: true,
    animateIn: 'fadeIn',
    animateOut: 'fadeOut',
    navText: [
      '<img class="slide-nav" src="./img/icon/icon-arrow-left-circle.svg" alt="">',
      '<img class="slide-nav" src="./img/icon/icon-arrow-right-circle.svg" alt="">',
    ],
    onInitialized: function (event) {
      updatePagination(event, '.js-list-group2-slide');
    },
    onChanged: function (event) {
      updatePagination(event, '.js-list-group2-slide');
    }
  });

  $('.js-company-slide').length && $('.js-company-slide').owlCarousel({
    items: 2,
    slideSpeed: 1000,
    autoplay: true,
    loop: true,
    smartSpeed: 1000,
    margin: 0,
    fluidSpeed: 500,
    autoplayTimeout: 5000,
    dots: true,
    nav: false,
    responsive: {
      576: {
        items: 2
      },
      992: {
        items: 3
      },
      1200: {
        items: 5
      }
    },
    animateIn: 'fadeIn',
    animateOut: 'fadeOut',
  });

}

function updatePagination(event, selector) {
  if (!event.namespace) return;
  var carousel = event.relatedTarget;
  var current = carousel.relative(carousel.current()) + 1;
  var total = carousel.items().length;
  $(selector).siblings('.custom-pagination')
    .text(current + '/' + total + ' trang');
}

function initDomEvent() {
  $('.btn-sidebar-trigger').on('click', e => {
    e.preventDefault();
    $('.dashboard-sidebar').toggleClass('active');
  })

  // handle-dropdown trigger show/hide
  $('.handle-dropdown').on('click', e => {
    e.preventDefault();
    $(e.currentTarget)
      .closest('.btn-dropdown-wrap')
      .toggleClass('active');
  });

  // mobile menu
  $('.icon-mobile-menu, .icon-mobile-close-menu').click(e => {
    $('.nav-mobile-wrap').toggleClass('active')
  })

  $('.toggle-menu-mobile-1').click(e => {
    let $menu = $($(e.target)).closest('.nav-mobile-1__item').find('.nav-mobile-2')
    let $icon = $($(e.target)).closest('.nav-mobile-1__item').find('.toggle-menu-mobile-1 img')
    $icon.toggleClass('active')
    $menu.toggleClass('active')
  })

  $('.toggle-menu-mobile-2').click(e => {
    let $menu = $($(e.target)).closest('.nav-mobile-2__item').find('.nav-mobile-3')
    let $icon = $($(e.target)).closest('.nav-mobile-2__item').find('.toggle-menu-mobile-2 img')
    $icon.toggleClass('active')
    $menu.toggleClass('active')
  })
  // end mobile menu

  $('.goto-id').click(function (e) {
    e.preventDefault(); // Ngăn chặn hành vi mặc định

    let target = $(this).attr('href'); // Lấy ID từ href
    if ($(target).length) {
      $('html, body').animate(
        {
          scrollTop: $(target).offset().top
        },
        500 // 0.5s
      );
    }
  });

// icon goto top
  $('.js-goto-top').click(e => {
    $("html, body").animate({scrollTop: 0}, 600);
    return false;
  })
  $(window).scroll(function () {
    if ($(this).scrollTop() >= 300) {        // If page is scrolled more than 50px
      $('.js-goto-top').addClass('active');    // Fade in the arrow
    } else {
      $('.js-goto-top').removeClass('active');   // Else fade out the arrow
    }
  });

//tabs click to active
  $('.tab').on('click', event => {
    const id = '#' + $(event.target).closest('.tab').attr('data-target');
    const triggerWrapId = '#' + $(event.target).data('trigger-target')

    if (triggerWrapId.length > 1) {
      const triggerWrapValue = $(event.target).data('trigger-value');
      if (triggerWrapValue && triggerWrapValue.length > 1) {
        $(triggerWrapId).attr('data-trigger-value', triggerWrapValue)
      }
    }

    $(id).parent().find('.tab-content').removeClass('active')
    $(id).addClass('active')
    $(event.target).closest('.tab').addClass('active')
    $(event.target).closest('.tab').siblings().removeClass('active')
  })

  $('[data-hide-when-click]').on('click', event => {
    $(event.target).addClass('d-none')
  })

  const scrollHorList = document.querySelectorAll(".js-scroll-hor");

  scrollHorList.forEach(container => {
    container.addEventListener("wheel", function (e) {
      if (e.deltaY > 0) {
        container.scrollLeft += 100;
      } else {
        container.scrollLeft -= 100;
      }
      e.preventDefault(); // preventDefault() will help avoid worrisome inclusion of vertical scroll
    });
  });
}
