<?php

/////////////////////////////////////
// Ensure excerpt function is available early
/////////////////////////////////////
if ( ! function_exists( 'excerpt' ) ) {
function excerpt($limit) {
  $excerpt = explode(' ', get_the_excerpt(), $limit);
  if (count($excerpt)>=$limit) {
    array_pop($excerpt);
    $excerpt = implode(" ",$excerpt).'...';
  } else {
    $excerpt = implode(" ",$excerpt);
  }
  $excerpt = preg_replace('`\[[^\]]*\]`','',$excerpt);
  return $excerpt;
}
}

if ( ! function_exists( 'content' ) ) {
function content($limit) {
  $content = explode(' ', get_the_content(), $limit);
  if (count($content)>=$limit) {
    array_pop($content);
    $content = implode(" ",$content).'...';
  } else {
    $content = implode(" ",$content);
  }
  $content = preg_replace('/\[.+\]/','', $content);
  $content = apply_filters('the_content', $content);
  $content = str_replace(']]>', ']]&gt;', $content);
  return $content;
}
}

/////////////////////////////////////
// Theme Setup
/////////////////////////////////////

if ( ! function_exists( 'mvp_setup' ) ) {
function mvp_setup(){
	load_theme_textdomain('mvp-text', get_template_directory() . '/languages');
	load_theme_textdomain('theia-post-slider', get_template_directory() . '/languages');
	load_theme_textdomain('framework_localize', get_template_directory() . '/languages');

	$locale = get_locale();
	$locale_file = get_template_directory() . "/languages/$locale.php";
	if ( is_readable( $locale_file ) )
		require_once( $locale_file );
	
	// Add theme support for title tag
	add_theme_support( 'title-tag' );
	
	// Add theme support for automatic feed links
	add_theme_support( 'automatic-feed-links' );
	
	// Add theme support for post thumbnails
	add_theme_support( 'post-thumbnails' );
}
}
add_action('after_setup_theme', 'mvp_setup');

/////////////////////////////////////
// Enqueue Javascript/CSS Files
/////////////////////////////////////

if ( ! function_exists( 'mvp_scripts_method' ) ) {
function mvp_scripts_method() {
	global $wp_styles;
	wp_enqueue_style( 'reset', get_template_directory_uri() . '/css/reset.css' );
	wp_enqueue_style( 'mvp-fontawesome', '//netdna.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.css' );
	wp_enqueue_style( 'mvp-style', get_stylesheet_uri() );
	wp_enqueue_style( 'iecss', get_stylesheet_directory_uri() . "/css/iecss.css", array( 'mvp-style' )  );
	$wp_styles->add_data( 'iecss', 'conditional', 'lt IE 10' );
	$mvp_respond = get_option('mvp_respond'); if ($mvp_respond == "true") { if (isset($mvp_respond)) {
	wp_enqueue_style( 'media-queries', get_template_directory_uri() . '/css/media-queries.css' );
	} }
	$mvp_skin_layout = get_option('mvp_skin_layout'); if ($mvp_skin_layout == "Entertainment") { if (isset($mvp_skin_layout)) {
	wp_enqueue_style( 'style-entertaiment', get_template_directory_uri() . '/css/style-ent.css' );
	} }
	$mvp_skin_layout = get_option('mvp_skin_layout'); if ($mvp_skin_layout == "Fashion") { if (isset($mvp_skin_layout)) {
	wp_enqueue_style( 'style-fashion', get_template_directory_uri() . '/css/style-fashion.css' );
	} }
	wp_enqueue_style( 'googlefonts', '//fonts.googleapis.com/css?family=Oswald:400,700|Open+Sans:300,400,600,700,800', array(), null, 'screen' );

	wp_register_script('devoe', get_template_directory_uri() . '/js/scripts.js', array('jquery'), '', true);
	wp_register_script('infinitescroll', get_template_directory_uri() . '/js/jquery.infinitescroll.min.js', array('jquery'), '', true);
	//wp_register_script('retina', get_template_directory_uri() . '/js/retina.js', array('jquery'), '', true);
	wp_register_script('nicescroll', get_template_directory_uri() . '/js/jquery.nicescroll.js', array('jquery'), '', true);

	wp_enqueue_script('devoe');
	wp_enqueue_script('nicescroll');

	$mvp_infinite_scroll = get_option('mvp_infinite_scroll'); if ($mvp_infinite_scroll == "true") { if (isset($mvp_infinite_scroll)) {
	wp_enqueue_script('infinitescroll');
	} }

	//wp_enqueue_script('retina');

}
}
add_action('wp_enqueue_scripts', 'mvp_scripts_method');

/////////////////////////////////////
// Google Fonts
/////////////////////////////////////

if ( ! function_exists( 'mvp_studio_fonts_url' ) ) {
function mvp_studio_fonts_url() {
    $font_url = '';

    if ( 'off' !== _x( 'on', 'Google font: on or off', 'studio' ) ) {
        $font_url = add_query_arg( 'family', urlencode( 'Oswald:400,700|Open+Sans:300,400,600,700,800&subset=latin,latin-ext' ), "//fonts.googleapis.com/css" );
    }
    return $font_url;
}
}

if ( ! function_exists( 'mvp_studio_scripts' ) ) {
function mvp_studio_scripts() {
    wp_enqueue_style( 'studio-fonts', mvp_studio_fonts_url(), array(), '1.0.0' );
}
}
add_action( 'wp_enqueue_scripts', 'mvp_studio_scripts' );

/////////////////////////////////////
// Theme Options
/////////////////////////////////////

require_once(TEMPLATEPATH . '/admin/admin-functions.php');
require_once(TEMPLATEPATH . '/admin/admin-interface.php');
require_once(TEMPLATEPATH . '/admin/theme-settings.php');

if ( ! function_exists( 'mvp_theme_options' ) ) {
function mvp_theme_options() {
	$wallad = get_option('mvp_wall_ad');
	$primarytheme = get_option('mvp_primary_theme');
	$accent1 = get_option('mvp_accent1');
	$accent2 = get_option('mvp_accent2');
	$topnavbg = get_option('mvp_top_nav_bg');
	$topnavtext = get_option('mvp_top_nav_text');
	$headlines = get_option('mvp_headlines');
	$link = get_option('mvp_link_color');
	$linkhover = get_option('mvp_link_hover');
	$featured_font = get_option('mvp_featured_font');
	$headline_font = get_option('mvp_headline_font');
	$heading_font = get_option('mvp_heading_font');
	$content_font = get_option('mvp_content_font');
	$menu_font = get_option('mvp_menu_font');
	$google_featured = preg_replace("/ /","+",$featured_font);
	$google_headlines = preg_replace("/ /","+",$headline_font);
	$google_heading = preg_replace("/ /","+",$heading_font);
	$google_content = preg_replace("/ /","+",$content_font);
	$google_menu = preg_replace("/ /","+",$menu_font);

	$mvp_skin_layout = get_option('mvp_skin_layout'); if ( $mvp_skin_layout == "Fashion" || $mvp_skin_layout == "Entertainment") { } else {
	echo "
<style type='text/css'>

@import url(//fonts.googleapis.com/css?family=$google_featured:100,200,300,400,500,600,700,800,900|$google_headlines:100,200,300,400,500,600,700,800,900|$google_heading:100,200,300,400,400italic,500,600,700,700italic,800,900|$google_content:100,200,300,400,400italic,500,600,700,700italic,800,900|$google_menu:100,200,300,400,500,600,700,800,900&subset=latin,latin-ext,cyrillic,cyrillic-ext,greek-ext,greek,vietnamese);

a,
a:visited {
	color: $link;
	}

.horz-list-head,
span.related-head,
span.post-header {
	background: $primarytheme;
	}

.woocommerce .widget_price_filter .price_slider_wrapper .ui-widget-content {
	background-color: $primarytheme;
	}

.horz-list-head:after {
	border-color: transparent transparent transparent $primarytheme;
	}

h4.post-header {
	border-bottom: 1px solid $primarytheme;
	}

span.post-header:after {
	border-color: $primarytheme transparent transparent transparent;
	}

span.post-cat a,
span.archive-list-cat,
span.post-tags-header,
.woocommerce .star-rating span,
.post-tags a:hover {
	color: $accent1;
	}

.woocommerce .widget_price_filter .ui-slider .ui-slider-range,
.woocommerce .widget_price_filter .ui-slider .ui-slider-handle {
	background-color: $accent1;
	}

.comment-reply a,
.feat-video {
	background: $accent1;
	}

.woocommerce span.onsale,
.woocommerce #respond input#submit.alt,
.woocommerce a.button.alt,
.woocommerce button.button.alt,
.woocommerce input.button.alt,
.woocommerce #respond input#submit.alt:hover,
.woocommerce a.button.alt:hover,
.woocommerce button.button.alt:hover,
.woocommerce input.button.alt:hover {
	background-color: $accent1;
	}

.feat-gallery {
	background: $accent2;
	}

#main-nav-wrap {
	background: $topnavbg;
	}

#main-nav-right ul.feat-trend-list li.trend-item a,
.small-nav-wrap ul li a {
	color: $topnavtext;
	}

@media screen and (max-width: 599px) {
.fly-but-wrap span {
	background: $topnavtext;
	}

.fly-but-wrap {
	background: $topnavbg;
	}
}

.head-right-wrap ul.feat-trend-list li.trend-item a,
.horz-list-text h2,
.archive-list-text a,
span.author-name a,
.prev-next-text,
.head-latest-text h2,
h2.author-list-head a {
	color: $headlines;
	}

.feat-main-top-text h2,
.feat-main-sub-text h2,
.feat-wide-text h2,
.feat-main2-text h2,
#woo-content h1.page-title,
.woocommerce div.product .product_title,
.woocommerce ul.products li.product h3 {
	font-family: '$featured_font', sans-serif;
	}

.head-latest-text h2,
ul.feat-trend-list li.trend-item a,
.horz-list-text h2,
.archive-list-text a,
.prev-next-text,
h1.post-title,
.content-main blockquote p,
.woocommerce ul.product_list_widget span.product-title,
.woocommerce ul.product_list_widget li a,
.woocommerce .related h2,
.woocommerce div.product .woocommerce-tabs .panel h2,
.feat-sub2-text a,.content-main h1,
.content-main h2,
.content-main h3,
.content-main h4,
.content-main h5,
.content-main h6,
h2.author-list-head {
	font-family: '$headline_font', sans-serif;
	}

span.related-head,
h1.cat-head,
span.cat-head-mobi,
span.head-latest-head,
ul.feat-trend-list li.trend-head,
h3.home-widget-head,
.horz-list-head h3,
span.post-header {
	font-family: '$heading_font', sans-serif;
	}

.head-latest-text p,
.archive-list-text p,
.feat-main-top-text p,
.feat-wide-text p,
span.post-excerpt p,
.content-main,
.author-page-text p,
#post-404,
.foot-widget,
.feat-main2-text p {
	font-family: '$content_font', sans-serif;
	}

nav.main-menu ul li a,
.small-nav-wrap ul li a,
.foot-menu ul.menu li a {
	font-family: '$menu_font', sans-serif;
	}

ul.head-latest-list li:hover .head-latest-text h2,
ul.feat-trend-list li.trend-item a:hover,
ul.horz-list li:hover .horz-list-text h2,
#main-nav-right ul.feat-trend-list li.trend-item a:hover,
.small-nav-wrap ul li a:hover,
.archive-list-text a:hover,
#foot-wrap a:hover,
.prev-next-item:hover .prev-next-text,
ul.author-social li a:hover,
span.author-name a:hover,
.woocommerce .sidebar-widget a:hover,
h2.author-list-head a:hover,
span.post-cat a:hover,
nav.main-menu ul li a:hover,
a:hover {
	color: $linkhover;
	}

</style>
	"; }
}
}
add_action( 'wp_head', 'mvp_theme_options' );

/////////////////////////////////////
// Register Widgets
/////////////////////////////////////

if ( !function_exists( 'mvp_sidebars_init' ) ) {
	function mvp_sidebars_init() {
		register_sidebar(array(
			'id' => 'homepage-widget',
			'name' => 'Homepage Widget Area',
			'before_widget' => '<div id="%1$s" class="home-widget-wrap %2$s"><div class="sec-marg-out relative"><div class="sec-marg-in">',
			'after_widget' => '</div></div></div>',
			'before_title' => '<h3 class="home-widget-head left relative">',
			'after_title' => '</h3>',
		));

		register_sidebar(array(
			'id' => 'sidebar-widget',
			'name' => 'Sidebar Widget Area',
			'before_widget' => '<div id="%1$s" class="sidebar-widget %2$s">',
			'after_widget' => '</div>',
			'before_title' => '<h4 class="post-header"><span class="post-header">',
			'after_title' => '</span></h4>',
		));

		register_sidebar(array(
			'id' => 'sidebar-widget-home',
			'name' => 'Home Sidebar Widget Area',
			'before_widget' => '<div id="%1$s" class="sidebar-widget %2$s">',
			'after_widget' => '</div>',
			'before_title' => '<h4 class="post-header"><span class="post-header">',
			'after_title' => '</span></h4>',
		));

		register_sidebar(array(
			'id' => 'sidebar-widget-post',
			'name' => 'Post/Page Sidebar Widget Area',
			'before_widget' => '<div id="%1$s" class="sidebar-widget %2$s">',
			'after_widget' => '</div>',
			'before_title' => '<h4 class="post-header"><span class="post-header">',
			'after_title' => '</span></h4>',
		));

		register_sidebar(array(
			'id' => 'footer-widget',
			'name' => 'Footer Widget Area',
			'before_widget' => '<div id="%1$s" class="foot-widget left relative %2$s">',
			'after_widget' => '</div>',
			'before_title' => '<h3 class="foot-head">',
			'after_title' => '</h3>',
		));

		register_sidebar(array(
			'id' => 'sidebar-woo-widget',
			'name' => 'WooCommerce Sidebar Widget Area',
			'before_widget' => '<div id="%1$s" class="sidebar-widget %2$s">',
			'after_widget' => '</div>',
			'before_title' => '<h4 class="post-header"><span class="post-header">',
			'after_title' => '</span></h4>',
		));

	}
}
add_action( 'widgets_init', 'mvp_sidebars_init' );

/////////////////////////////////////
// Widget Loading and Registration
/////////////////////////////////////

// Register the improved widget
add_action( 'widgets_init', 'mvp_catlist_load_widgets' );
function mvp_catlist_load_widgets() {
    register_widget( 'mvp_catlist_widget' );
}

class mvp_catlist_widget extends WP_Widget {
    /**
     * Widget setup.
     */
    public function __construct() {
        /* Widget settings. */
        $widget_ops = array( 
            'classname' => 'mvp_catlist_widget', 
            'description' => __('A widget that displays a list of posts from a category of your choice.', 'mvp_catlist_widget') 
        );

        /* Widget control settings. */
        $control_ops = array( 
            'width' => 250, 
            'height' => 350, 
            'id_base' => 'mvp_catlist_widget' 
        );

        /* Create the widget. */
        parent::__construct( 
            'mvp_catlist_widget', 
            __('DeVoe: Homepage Category List Widget', 'mvp_catlist_widget'), 
            $widget_ops, 
            $control_ops 
        );
    }

    /**
     * How to display the widget on the screen.
     */
    public function widget( $args, $instance ) {
        /* Our variables from the widget settings. */
        global $post;
        $before_widget = isset($args['before_widget']) ? $args['before_widget'] : '';
        $after_widget = isset($args['after_widget']) ? $args['after_widget'] : '';
        
        $title = apply_filters('widget_title', isset($instance['title']) ? $instance['title'] : '');
        $categories = isset($instance['categories']) ? $instance['categories'] : '';
        $layout = isset($instance['layout']) ? $instance['layout'] : 'row';
        $showcat = isset($instance['showcat']) ? $instance['showcat'] : 'on';
        $number = isset($instance['number']) ? $instance['number'] : 5;
        $code = isset($instance['code']) ? $instance['code'] : '';

        /* Before widget (defined by themes). */
        echo $before_widget;

        ?>
        <div class="archive-list-wrap left relative">
            <?php if($code) { ?>
                <div class="archive-list-out">
                    <div class="archive-list-in">
                        <div class="archive-list-left left relative">
                            <?php if ( $title ) { ?><h3 class="home-widget-head"><?php echo esc_html( $title ); ?></h3><?php } ?>
                            <?php if($layout == 'column') { ?>
                            <ul class="archive-col">
                                <?php 
                                $query_args = array( 
                                    'cat' => $categories, 
                                    'posts_per_page' => $number 
                                );
                                $recent = new WP_Query($query_args); 
                                while($recent->have_posts()) : $recent->the_post(); ?>
                                <li>
                                    <?php if (  (function_exists('has_post_thumbnail')) && (has_post_thumbnail())  ) { ?>
                                        <div class="archive-list-img left relative">
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark" title="<?php echo esc_attr(get_the_title()); ?>">
                                            <?php the_post_thumbnail('medium-thumb'); ?>
                                            <?php global $numpages; if(get_post_meta($post->ID, "mvp_video_embed", true)) { ?>
                                                <div class="feat-video">
                                                    <i class="fa fa-video-camera fa-2"></i>
                                                </div><!--feat-video-->
                                            <?php } else if ( $numpages > 1 ) { ?>
                                                <div class="feat-gallery">
                                                    <i class="fa fa-camera-retro fa-2"></i>
                                                </div><!--feat-gallery-->
                                            <?php } ?>
                                            </a>
                                            <div class="archive-list-share">
                                                <span class="archive-share-but left"><i class="fa fa-share-square-o fa-2"></i></span>
                                                <div class="archive-share-contain left relative">
                                                    <span class="archive-share-fb"><a href="#" onclick="window.open('<?php echo esc_js('http://www.facebook.com/sharer.php?u=' . get_permalink() . '&amp;t=' . get_the_title()); ?>', 'facebookShare', 'width=626,height=436'); return false;" title="Share on Facebook"><i class="fa fa-facebook fa-2"></i></a></span><span class="archive-share-twit"><a href="#" onclick="window.open('<?php echo esc_js('http://twitter.com/share?text=' . get_the_title() . '&amp;url=' . get_permalink()); ?>', 'twitterShare', 'width=626,height=436'); return false;" title="Tweet This Post"><i class="fa fa-twitter fa-2"></i></a></span><span class="archive-share-pin"><a href="#" onclick="window.open('<?php $thumb = wp_get_attachment_image_src( get_post_thumbnail_id($post->ID), 'post-thumb' ); echo esc_js('http://pinterest.com/pin/create/button/?url=' . get_permalink() . '&amp;media=' . (isset($thumb[0]) ? $thumb[0] : '') . '&amp;description=' . get_the_title()); ?>', 'pinterestShare', 'width=750,height=350'); return false;" title="Pin This Post"><i class="fa fa-pinterest fa-2"></i></a></span>
                                                </div><!--archive-share-contain-->
                                            </div><!--archive-list-share-->
                                        </div><!--archive-list-img-->
                                        <div class="archive-list-text left relative">
                                            <?php if($showcat) { ?>
                                                <div class="archive-list-info left relative">
                                                    <span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span>
                                                </div><!--archive-list-info-->
                                            <?php } ?>
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                            <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                            <div class="archive-list-info left relative">
                                                <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                            </div><!--archive-list-info-->
                                        </div><!--archive-list-text-->
                                    <?php } else { ?>
                                        <div class="archive-list-text left relative w100">
                                            <?php if($showcat) { ?>
                                                <div class="archive-list-info left relative">
                                                    <span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span>
                                                </div><!--archive-list-info-->
                                            <?php } ?>
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                            <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                            <div class="archive-list-info left relative">
                                                <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                            </div><!--archive-list-info-->
                                        </div><!--archive-list-text-->
                                    <?php } ?>
                                </li>
                                <?php endwhile; wp_reset_postdata(); ?>
                            </ul>
                            <?php } else { ?>
                            <ul class="archive-list">
                                <?php 
                                $query_args = array( 
                                    'cat' => $categories, 
                                    'posts_per_page' => $number 
                                );
                                $recent = new WP_Query($query_args); 
                                while($recent->have_posts()) : $recent->the_post(); ?>
                                <li>
                                    <?php if (  (function_exists('has_post_thumbnail')) && (has_post_thumbnail())  ) { ?>
                                        <div class="archive-list-img left relative">
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark" title="<?php echo esc_attr(get_the_title()); ?>">
                                            <?php the_post_thumbnail('medium-thumb'); ?>
                                            <?php global $numpages; if(get_post_meta($post->ID, "mvp_video_embed", true)) { ?>
                                                <div class="feat-video">
                                                    <i class="fa fa-video-camera fa-2"></i>
                                                </div><!--feat-video-->
                                            <?php } else if ( $numpages > 1 ) { ?>
                                                <div class="feat-gallery">
                                                    <i class="fa fa-camera-retro fa-2"></i>
                                                </div><!--feat-gallery-->
                                            <?php } ?>
                                            </a>
                                            <div class="archive-list-share">
                                                <span class="archive-share-but left"><i class="fa fa-share-square-o fa-2"></i></span>
                                                <div class="archive-share-contain left relative">
                                                    <span class="archive-share-fb"><a href="#" onclick="window.open('<?php echo esc_js('http://www.facebook.com/sharer.php?u=' . get_permalink() . '&amp;t=' . get_the_title()); ?>', 'facebookShare', 'width=626,height=436'); return false;" title="Share on Facebook"><i class="fa fa-facebook fa-2"></i></a></span><span class="archive-share-twit"><a href="#" onclick="window.open('<?php echo esc_js('http://twitter.com/share?text=' . get_the_title() . '&amp;url=' . get_permalink()); ?>', 'twitterShare', 'width=626,height=436'); return false;" title="Tweet This Post"><i class="fa fa-twitter fa-2"></i></a></span><span class="archive-share-pin"><a href="#" onclick="window.open('<?php $thumb = wp_get_attachment_image_src( get_post_thumbnail_id($post->ID), 'post-thumb' ); echo esc_js('http://pinterest.com/pin/create/button/?url=' . get_permalink() . '&amp;media=' . (isset($thumb[0]) ? $thumb[0] : '') . '&amp;description=' . get_the_title()); ?>', 'pinterestShare', 'width=750,height=350'); return false;" title="Pin This Post"><i class="fa fa-pinterest fa-2"></i></a></span>
                                                </div><!--archive-share-contain-->
                                            </div><!--archive-list-share-->
                                        </div><!--archive-list-img-->
                                        <div class="archive-list-text left relative">
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                            <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                            <div class="archive-list-info left relative">
                                                <?php if($showcat) { ?><span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span><?php } ?><span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                            </div><!--archive-list-info-->
                                        </div><!--archive-list-text-->
                                    <?php } else { ?>
                                        <div class="archive-list-text left relative w100">
                                            <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                            <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                            <div class="archive-list-info left relative">
                                                <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                            </div><!--archive-list-info-->
                                        </div><!--archive-list-text-->
                                    <?php } ?>
                                </li>
                                <?php endwhile; wp_reset_postdata(); ?>
                            </ul>
                            <?php } ?>
                        </div><!--archive-list-left-->
                    </div><!--archive-list-in-->
                    <div class="archive-ad-wrap relative">
                        <?php echo wp_kses_post($code); ?>
                    </div><!--archive-ad-wrap-->
                </div><!--archive-list-out-->
            <?php } else { ?>
                <div class="archive-list-left left relative">
                    <?php if ( $title ) { ?><h3 class="home-widget-head"><?php echo esc_html( $title ); ?></h3><?php } ?>
                    <?php if($layout == 'column') { ?>
                    <ul class="archive-col">
                        <?php 
                        $query_args = array( 
                            'cat' => $categories, 
                            'posts_per_page' => $number 
                        );
                        $recent = new WP_Query($query_args); 
                        while($recent->have_posts()) : $recent->the_post(); ?>
                        <li>
                            <?php if (  (function_exists('has_post_thumbnail')) && (has_post_thumbnail())  ) { ?>
                                <div class="archive-list-img left relative">
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark" title="<?php echo esc_attr(get_the_title()); ?>">
                                    <?php the_post_thumbnail('medium-thumb'); ?>
                                    </a>
                                </div><!--archive-list-img-->
                                <div class="archive-list-text left relative">
                                    <?php if($showcat) { ?>
                                        <div class="archive-list-info left relative">
                                            <span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span>
                                        </div><!--archive-list-info-->
                                    <?php } ?>
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                    <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                    <div class="archive-list-info left relative">
                                        <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                    </div><!--archive-list-info-->
                                </div><!--archive-list-text-->
                            <?php } else { ?>
                                <div class="archive-list-text left relative w100">
                                    <?php if($showcat) { ?>
                                        <div class="archive-list-info left relative">
                                            <span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span>
                                        </div><!--archive-list-info-->
                                    <?php } ?>
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                    <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                    <div class="archive-list-info left relative">
                                        <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                    </div><!--archive-list-info-->
                                </div><!--archive-list-text-->
                            <?php } ?>
                        </li>
                        <?php endwhile; wp_reset_postdata(); ?>
                    </ul>
                    <?php } else { ?>
                    <ul class="archive-list">
                        <?php 
                        $query_args = array( 
                            'cat' => $categories, 
                            'posts_per_page' => $number 
                        );
                        $recent = new WP_Query($query_args); 
                        while($recent->have_posts()) : $recent->the_post(); ?>
                        <li>
                            <?php if (  (function_exists('has_post_thumbnail')) && (has_post_thumbnail())  ) { ?>
                                <div class="archive-list-img left relative">
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark" title="<?php echo esc_attr(get_the_title()); ?>">
                                    <?php the_post_thumbnail('medium-thumb'); ?>
                                    </a>
                                </div><!--archive-list-img-->
                                <div class="archive-list-text left relative">
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                    <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                    <div class="archive-list-info left relative">
                                        <?php if($showcat) { ?><span class="archive-list-cat left"><?php $category = get_the_category(); echo esc_html( isset($category[0]) ? $category[0]->cat_name : '' ); ?></span><?php } ?><span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                    </div><!--archive-list-info-->
                                </div><!--archive-list-text-->
                            <?php } else { ?>
                                <div class="archive-list-text left relative w100">
                                    <a href="<?php echo esc_url(get_permalink()); ?>" rel="bookmark"><?php echo esc_html(get_the_title()); ?></a>
                                    <p><?php echo esc_html(wp_trim_words(get_the_excerpt(), 22)); ?></p>
                                    <div class="archive-list-info left relative">
                                        <span class="archive-list-author left"><?php echo esc_html(get_the_author()); ?></span><span class="archive-list-date left"><?php echo esc_html(get_the_time(get_option('date_format'))); ?></span>
                                    </div><!--archive-list-info-->
                                </div><!--archive-list-text-->
                            <?php } ?>
                        </li>
                        <?php endwhile; wp_reset_postdata(); ?>
                    </ul>
                    <?php } ?>
                </div><!--archive-list-left-->
            <?php } ?>
        </div><!--archive-list-wrap-->

        <?php
        /* After widget (defined by themes). */
        echo $after_widget;
    }

    /**
     * Update the widget settings.
     */
    public function update( $new_instance, $old_instance ) {
        $instance = $old_instance;

        /* Strip tags for title and name to remove HTML (important for text inputs). */
        $instance['title'] = strip_tags( isset($new_instance['title']) ? $new_instance['title'] : '' );
        $instance['categories'] = strip_tags( isset($new_instance['categories']) ? $new_instance['categories'] : '' );
        $instance['layout'] = strip_tags( isset($new_instance['layout']) ? $new_instance['layout'] : 'row' );
        $instance['showcat'] = strip_tags( isset($new_instance['showcat']) ? $new_instance['showcat'] : 'on' );
        $instance['number'] = strip_tags( isset($new_instance['number']) ? $new_instance['number'] : 5 );
        $instance['code'] = isset($new_instance['code']) ? $new_instance['code'] : '';

        return $instance;
    }

    /**
     * Display the widget form in admin.
     */
    public function form( $instance ) {
        /* Set up some default widget settings. */
        $defaults = array( 
            'title' => 'Title', 
            'categories' => '', 
            'layout' => 'row', 
            'showcat' => 'on', 
            'number' => 5,
            'code' => ''
        );
        $instance = wp_parse_args( (array) $instance, $defaults ); ?>

        <!-- Widget Title: Text Input -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id( 'title' )); ?>">Title:</label>
            <input id="<?php echo esc_attr($this->get_field_id( 'title' )); ?>" name="<?php echo esc_attr($this->get_field_name( 'title' )); ?>" value="<?php echo esc_attr($instance['title']); ?>" style="width:90%;" />
        </p>
<!-- Category -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id('categories')); ?>">Select category:</label>
            <select id="<?php echo esc_attr($this->get_field_id('categories')); ?>" name="<?php echo esc_attr($this->get_field_name('categories')); ?>" style="width:100%;">
                <option value='' <?php selected( $instance['categories'], '' ); ?>>All Categories</option>
                <?php $categories = get_categories('hide_empty=0&depth=1&type=post'); ?>
                <?php foreach($categories as $category) {  ?>
                <option value='<?php echo esc_attr($category->term_id); ?>' <?php selected( $instance['categories'], $category->term_id ); ?>><?php echo esc_html($category->cat_name); ?></option>
                <?php } ?>
            </select>
        </p>

        <!-- Layout -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id('layout')); ?>">Select layout:</label>
            <select id="<?php echo esc_attr($this->get_field_id('layout')); ?>" name="<?php echo esc_attr($this->get_field_name('layout')); ?>" style="width:100%;">
                <option value='row' <?php selected( $instance['layout'], 'row' ); ?>>Row</option>
                <option value='column' <?php selected( $instance['layout'], 'column' ); ?>>Column</option>
            </select>
        </p>

        <!-- Show Categories -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id( 'showcat' )); ?>">Show categories on posts:</label>
            <input type="checkbox" id="<?php echo esc_attr($this->get_field_id( 'showcat' )); ?>" name="<?php echo esc_attr($this->get_field_name( 'showcat' )); ?>" <?php checked( (bool) $instance['showcat'], true ); ?> />
        </p>

        <!-- Number of posts -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id( 'number' )); ?>">Number of posts to display:</label>
            <input id="<?php echo esc_attr($this->get_field_id( 'number' )); ?>" name="<?php echo esc_attr($this->get_field_name( 'number' )); ?>" value="<?php echo esc_attr($instance['number']); ?>" size="3" />
        </p>

        <!-- Ad code -->
        <p>
            <label for="<?php echo esc_attr($this->get_field_id( 'code' )); ?>">Ad code:</label>
            <textarea id="<?php echo esc_attr($this->get_field_id( 'code' )); ?>" name="<?php echo esc_attr($this->get_field_name( 'code' )); ?>" style="width:96%;" rows="6"><?php echo esc_textarea($instance['code']); ?></textarea>
        </p>

    <?php
    }
}

// Load other widget files with PHP 8.1 compatibility
include_once("widgets/widget-ad.php");
include_once("widgets/widget-facebook.php");
include_once("widgets/widget-feat.php");
include_once("widgets/widget-sidecat.php");
include_once("widgets/widget-taglist.php");
include_once("widgets/widget-tagrow.php");
include_once("widgets/widget-tags.php");

/////////////////////////////////////
// Register Custom Menus
/////////////////////////////////////

if ( !function_exists( 'register_menus' ) ) {
function register_menus() {
	register_nav_menus(
		array(
			'main-menu' => __( 'Main Menu', 'mvp-text' ),
			'nav-menu' => __( 'Nav Menu', 'mvp-text' ),
			'footer-menu' => __( 'Footer Menu', 'mvp-text' ))
	  	);
	  }
}
add_action( 'init', 'register_menus' );

/////////////////////////////////////
// Register Custom Background
/////////////////////////////////////

$custombg = array(
	'default-color' => 'ffffff',
);
add_theme_support( 'custom-background', $custombg );

/////////////////////////////////////
// Register Thumbnails
/////////////////////////////////////

if ( function_exists( 'add_theme_support' ) ) {
set_post_thumbnail_size( 1000, 600, true );
add_image_size( 'post-thumb', 1000, 600, true );
add_image_size( 'medium-thumb', 450, 270, true );
add_image_size( 'small-thumb', 150, 90, true );
}

/////////////////////////////////////
// Title Meta Data
/////////////////////////////////////

function mvp_filter_home_title(){
if ( ( is_home() && ! is_front_page() ) || ( ! is_home() && is_front_page() ) ) {
    $mvpHomeTitle = get_bloginfo( 'name', 'display' );
    $mvpHomeDesc = get_bloginfo( 'description', 'display' );
    return $mvpHomeTitle . " - " . $mvpHomeDesc;
}
}
add_filter( 'pre_get_document_title', 'mvp_filter_home_title');

/////////////////////////////////////
// Add Custom Meta Box
/////////////////////////////////////

/* Fire our meta box setup function on the post editor screen. */
add_action( 'load-post.php', 'mvp_post_meta_boxes_setup' );
add_action( 'load-post-new.php', 'mvp_post_meta_boxes_setup' );

/* Meta box setup function. */
if ( !function_exists( 'mvp_post_meta_boxes_setup' ) ) {
function mvp_post_meta_boxes_setup() {

	/* Add meta boxes on the 'add_meta_boxes' hook. */
	add_action( 'add_meta_boxes', 'mvp_add_post_meta_boxes' );

	/* Save post meta on the 'save_post' hook. */
	add_action( 'save_post', 'mvp_save_video_embed_meta', 10, 2 );
	add_action( 'save_post', 'mvp_save_featured_headline_meta', 10, 2 );
	add_action( 'save_post', 'mvp_save_photo_credit_meta', 10, 2 );
	add_action( 'save_post', 'mvp_save_post_template_meta', 10, 2 );
	add_action( 'save_post', 'mvp_save_featured_image_meta', 10, 2 );
}
}

/* Create one or more meta boxes to be displayed on the post editor screen. */
if ( !function_exists( 'mvp_add_post_meta_boxes' ) ) {
function mvp_add_post_meta_boxes() {

	add_meta_box(
		'mvp-video-embed',			// Unique ID
		esc_html__( 'Video/Audio Embed', 'mvp-text' ),		// Title
		'mvp_video_embed_meta_box',		// Callback function
		'post',					// Admin page (or post type)
		'normal',				// Context
		'high'					// Priority
	);

	add_meta_box(
		'mvp-featured-headline',			// Unique ID
		esc_html__( 'Featured Headline', 'mvp-text' ),		// Title
		'mvp_featured_headline_meta_box',		// Callback function
		'post',					// Admin page (or post type)
		'normal',				// Context
		'high'					// Priority
	);

	add_meta_box(
		'mvp-photo-credit',			// Unique ID
		esc_html__( 'Featured Image Caption', 'mvp-text' ),		// Title
		'mvp_photo_credit_meta_box',		// Callback function
		'post',					// Admin page (or post type)
		'normal',				// Context
		'high'					// Priority
	);

	add_meta_box(
		'mvp-post-template',			// Unique ID
		esc_html__( 'Post Template', 'mvp-text' ),		// Title
		'mvp_post_template_meta_box',		// Callback function
		'post',					// Admin page (or post type)
		'side',					// Context
		'core'					// Priority
	);

	add_meta_box(
		'mvp-featured-image',			// Unique ID
		esc_html__( 'Featured Image Show/Hide', 'mvp-text' ),		// Title
		'mvp_featured_image_meta_box',		// Callback function
		'post',					// Admin page (or post type)
		'side',					// Context
		'core'					// Priority
	);
}
}

/* Display the post meta box. */
if ( !function_exists( 'mvp_featured_headline_meta_box' ) ) {
function mvp_featured_headline_meta_box( $object, $box ) { ?>

	<?php wp_nonce_field( basename( __FILE__ ), 'mvp_featured_headline_nonce' ); ?>

	<p>
		<label for="mvp-featured-headline"><?php _e( "Add a custom featured headline that will be displayed in the featured slider.", 'example' ); ?></label>
		<br />
		<input class="widefat" type="text" name="mvp-featured-headline" id="mvp-featured-headline" value="<?php echo esc_html__( get_post_meta( $object->ID, 'mvp_featured_headline', true ) ); ?>" size="30" />
	</p>

<?php }
}

/* Display the post meta box. */
if ( !function_exists( 'mvp_video_embed_meta_box' ) ) {
function mvp_video_embed_meta_box( $object, $box ) { ?>

	<?php wp_nonce_field( basename( __FILE__ ), 'mvp_video_embed_nonce' ); ?>

	<p>
		<label for="mvp-video-embed"><?php _e( "Enter your video or audio embed code.", 'mvp-text' ); ?></label>
		<br />
		<input class="widefat" type="text" name="mvp-video-embed" id="mvp-video-embed" value="<?php echo esc_html__( get_post_meta( $object->ID, 'mvp_video_embed', true ) ); ?>" />
	</p>

<?php }
}

/* Display the post meta box. */
if ( !function_exists( 'mvp_photo_credit_meta_box' ) ) {
function mvp_photo_credit_meta_box( $object, $box ) { ?>

	<?php wp_nonce_field( basename( __FILE__ ), 'mvp_photo_credit_nonce' ); ?>

	<p>
		<label for="mvp-photo-credit"><?php _e( "Add a caption and/or photo credit information for the featured image.", 'mvp-text' ); ?></label>
		<br />
		<input class="widefat" type="text" name="mvp-photo-credit" id="mvp-photo-credit" value="<?php echo esc_html__( get_post_meta( $object->ID, 'mvp_photo_credit', true ) ); ?>" size="30" />
	</p>

<?php }
}

/* Display the post meta box. */
if ( !function_exists( 'mvp_post_template_meta_box' ) ) {
function mvp_post_template_meta_box( $object, $box ) { ?>

	<?php wp_nonce_field( basename( __FILE__ ), 'mvp_post_template_nonce' ); $selected = esc_html__( get_post_meta( $object->ID, 'mvp_post_template', true ) ); ?>

	<p>
		<label for="mvp-post-template"><?php _e( "Select a template for your post.", 'mvp-text' ); ?></label>
		<br /><br />
		<select class="widefat" name="mvp-post-template" id="mvp-post-template">
            		<option value="def" <?php selected( $selected, 'def' ); ?>>Default</option>
            		<option value="full" <?php selected( $selected, 'full' ); ?>>Default Full-Width</option>
			<option value="wide" <?php selected( $selected, 'wide' ); ?>>Wide Image</option>
			<option value="wide-full" <?php selected( $selected, 'wide-full' ); ?>>Wide Image Full-Width</option>
        	</select>
	</p>
<?php }
}

/* Display the post meta box. */
if ( !function_exists( 'mvp_featured_image_meta_box' ) ) {
function mvp_featured_image_meta_box( $object, $box ) { ?>

	<?php wp_nonce_field( basename( __FILE__ ), 'mvp_featured_image_nonce' ); $selected = esc_html__( get_post_meta( $object->ID, 'mvp_featured_image', true ) ); ?>

	<p>
		<label for="mvp-featured-image"><?php _e( "Select to show or hide the featured image from automatically displaying in this post.", 'mvp-text' ); ?></label>
		<br /><br />
		<select class="widefat" name="mvp-featured-image" id="mvp-featured-image">
            		<option value="show" <?php selected( $selected, 'show' ); ?>>Show</option>
            		<option value="hide" <?php selected( $selected, 'hide' ); ?>>Hide</option>
        	</select>
	</p>
<?php }
}

/* Save the meta box's post metadata. */
if ( !function_exists( 'mvp_save_video_embed_meta' ) ) {
function mvp_save_video_embed_meta( $post_id, $post ) {

	/* Verify the nonce before proceeding. */
	if ( !isset( $_POST['mvp_video_embed_nonce'] ) || !wp_verify_nonce( $_POST['mvp_video_embed_nonce'], basename( __FILE__ ) ) )
		return $post_id;

	/* Get the post type object. */
	$post_type = get_post_type_object( $post->post_type );

	/* Check if the current user has permission to edit the post. */
	if ( !current_user_can( $post_type->cap->edit_post, $post_id ) )
		return $post_id;

	/* Get the posted data and sanitize it for use as an HTML class. */
	$new_meta_value = ( isset( $_POST['mvp-video-embed'] ) ? balanceTags( $_POST['mvp-video-embed'] ) : '' );

	/* Get the meta key. */
	$meta_key = 'mvp_video_embed';

	/* Get the meta value of the custom field key. */
	$meta_value = get_post_meta( $post_id, $meta_key, true );

	/* If a new meta value was added and there was no previous value, add it. */
	if ( $new_meta_value && '' == $meta_value )
		add_post_meta( $post_id, $meta_key, $new_meta_value, true );

	/* If the new meta value does not match the old value, update it. */
	elseif ( $new_meta_value && $new_meta_value != $meta_value )
		update_post_meta( $post_id, $meta_key, $new_meta_value );

	/* If there is no new meta value but an old value exists, delete it. */
	elseif ( '' == $new_meta_value && $meta_value )
		delete_post_meta( $post_id, $meta_key, $meta_value );
} }

/* Save the meta box's post metadata. */
if ( !function_exists( 'mvp_save_featured_image_meta' ) ) {
function mvp_save_featured_image_meta( $post_id, $post ) {

	/* Verify the nonce before proceeding. */
	if ( !isset( $_POST['mvp_featured_image_nonce'] ) || !wp_verify_nonce( $_POST['mvp_featured_image_nonce'], basename( __FILE__ ) ) )
		return $post_id;

	/* Get the post type object. */
	$post_type = get_post_type_object( $post->post_type );

	/* Check if the current user has permission to edit the post. */
	if ( !current_user_can( $post_type->cap->edit_post, $post_id ) )
		return $post_id;

	/* Get the posted data and sanitize it for use as an HTML class. */
	$new_meta_value = ( isset( $_POST['mvp-featured-image'] ) ? balanceTags( $_POST['mvp-featured-image'] ) : '' );

	/* Get the meta key. */
	$meta_key = 'mvp_featured_image';

	/* Get the meta value of the custom field key. */
	$meta_value = get_post_meta( $post_id, $meta_key, true );

	/* If a new meta value was added and there was no previous value, add it. */
	if ( $new_meta_value && '' == $meta_value )
		add_post_meta( $post_id, $meta_key, $new_meta_value, true );

	/* If the new meta value does not match the old value, update it. */
	elseif ( $new_meta_value && $new_meta_value != $meta_value )
		update_post_meta( $post_id, $meta_key, $new_meta_value );

	/* If there is no new meta value but an old value exists, delete it. */
	elseif ( '' == $new_meta_value && $meta_value )
		delete_post_meta( $post_id, $meta_key, $meta_value );
} }

/* Save the meta box's post metadata. */
if ( !function_exists( 'mvp_save_featured_headline_meta' ) ) {
function mvp_save_featured_headline_meta( $post_id, $post ) {

	/* Verify the nonce before proceeding. */
	if ( !isset( $_POST['mvp_featured_headline_nonce'] ) || !wp_verify_nonce( $_POST['mvp_featured_headline_nonce'], basename( __FILE__ ) ) )
		return $post_id;

	/* Get the post type object. */
	$post_type = get_post_type_object( $post->post_type );

	/* Check if the current user has permission to edit the post. */
	if ( !current_user_can( $post_type->cap->edit_post, $post_id ) )
		return $post_id;

	/* Get the posted data and sanitize it for use as an HTML class. */
	$new_meta_value = ( isset( $_POST['mvp-featured-headline'] ) ? sanitize_text_field( $_POST['mvp-featured-headline'] ) : '' );

	/* Get the meta key. */
	$meta_key = 'mvp_featured_headline';

	/* Get the meta value of the custom field key. */
	$meta_value = get_post_meta( $post_id, $meta_key, true );

	/* If a new meta value was added and there was no previous value, add it. */
	if ( $new_meta_value && '' == $meta_value )
		add_post_meta( $post_id, $meta_key, $new_meta_value, true );

	/* If the new meta value does not match the old value, update it. */
	elseif ( $new_meta_value && $new_meta_value != $meta_value )
		update_post_meta( $post_id, $meta_key, $new_meta_value );

	/* If there is no new meta value but an old value exists, delete it. */
	elseif ( '' == $new_meta_value && $meta_value )
		delete_post_meta( $post_id, $meta_key, $meta_value );
} }

/* Save the meta box's post metadata. */
if ( !function_exists( 'mvp_save_photo_credit_meta' ) ) {
function mvp_save_photo_credit_meta( $post_id, $post ) {

	/* Verify the nonce before proceeding. */
	if ( !isset( $_POST['mvp_photo_credit_nonce'] ) || !wp_verify_nonce( $_POST['mvp_photo_credit_nonce'], basename( __FILE__ ) ) )
		return $post_id;

	/* Get the post type object. */
	$post_type = get_post_type_object( $post->post_type );

	/* Check if the current user has permission to edit the post. */
	if ( !current_user_can( $post_type->cap->edit_post, $post_id ) )
		return $post_id;

	/* Get the posted data and sanitize it for use as an HTML class. */
	$new_meta_value = ( isset( $_POST['mvp-photo-credit'] ) ? sanitize_text_field( $_POST['mvp-photo-credit'] ) : '' );

	/* Get the meta key. */
	$meta_key = 'mvp_photo_credit';

	/* Get the meta value of the custom field key. */
	$meta_value = get_post_meta( $post_id, $meta_key, true );

	/* If a new meta value was added and there was no previous value, add it. */
	if ( $new_meta_value && '' == $meta_value )
		add_post_meta( $post_id, $meta_key, $new_meta_value, true );

	/* If the new meta value does not match the old value, update it. */
	elseif ( $new_meta_value && $new_meta_value != $meta_value )
		update_post_meta( $post_id, $meta_key, $new_meta_value );

	/* If there is no new meta value but an old value exists, delete it. */
	elseif ( '' == $new_meta_value && $meta_value )
		delete_post_meta( $post_id, $meta_key, $meta_value );
} }

/* Save the meta box's post metadata. */
if ( !function_exists( 'mvp_save_post_template_meta' ) ) {
function mvp_save_post_template_meta( $post_id, $post ) {

	/* Verify the nonce before proceeding. */
	if ( !isset( $_POST['mvp_post_template_nonce'] ) || !wp_verify_nonce( $_POST['mvp_post_template_nonce'], basename( __FILE__ ) ) )
		return $post_id;

	/* Get the post type object. */
	$post_type = get_post_type_object( $post->post_type );

	/* Check if the current user has permission to edit the post. */
	if ( !current_user_can( $post_type->cap->edit_post, $post_id ) )
		return $post_id;

	/* Get the posted data and sanitize it for use as an HTML class. */
	$new_meta_value = ( isset( $_POST['mvp-post-template'] ) ? sanitize_text_field( $_POST['mvp-post-template'] ) : '' );

	/* Get the meta key. */
	$meta_key = 'mvp_post_template';

	/* Get the meta value of the custom field key. */
	$meta_value = get_post_meta( $post_id, $meta_key, true );

	/* If a new meta value was added and there was no previous value, add it. */
	if ( $new_meta_value && '' == $meta_value )
		add_post_meta( $post_id, $meta_key, $new_meta_value, true );

	/* If the new meta value does not match the old value, update it. */
	elseif ( $new_meta_value && $new_meta_value != $meta_value )
		update_post_meta( $post_id, $meta_key, $new_meta_value );

	/* If there is no new meta value but an old value exists, delete it. */
	elseif ( '' == $new_meta_value && $meta_value )
		delete_post_meta( $post_id, $meta_key, $meta_value );
} }

/////////////////////////////////////
// Social Shares
/////////////////////////////////////

if (!function_exists('get_tweets')) {
function get_tweets( $post_id ) {

    // Do API call
    $response = wp_remote_retrieve_body( wp_remote_get( 'https://cdn.api.twitter.com/1/urls/count.json?url=' . urlencode( get_permalink( $post_id ) ), array(

		'sslverify' => false,
		'compress' => true,
		'decompress' => false,
		'timeout' => 1

	) ) );

    // If error in API call, stop and don't store transient
    if ( is_wp_error( $response ) )
      return 'error';

    // Decode JSON
    $json = json_decode( $response, true );

    // Set total count
    $count = absint( $json['count'] );

 	return absint( $count );
} }

if (!function_exists('get_fb')) {
function get_fb( $post_id ) {

    // Do API call
    $response = wp_remote_retrieve_body( wp_remote_get( 'http://api.facebook.com/restserver.php?method=links.getStats&format=json&urls=' . urlencode( get_permalink( $post_id ) ), array(

		'sslverify' => false,
		'compress' => true,
		'decompress' => false,
		'timeout' => 5

	) ) );

    // If error in API call, stop and don't store transient
    if ( is_wp_error( $response ) )
      return 'error';

    // Decode JSON
    $json = json_decode( $response, true );

    // Set total count
    if(isset($json[0])){
    $count = absint( $json[0]['total_count'] );
	} else { }

 return absint( $count );
} }

if (!function_exists('get_pinterest')) {
function get_pinterest( $post_id ) {

    // Do API call
    $response = wp_remote_retrieve_body( wp_remote_get( 'http://api.pinterest.com/v1/urls/count.json?url=' . urlencode( get_permalink( $post_id ) ), array(

		'sslverify' => false,
		'compress' => true,
		'decompress' => false,
		'timeout' => 5

	) ) );

    // If error in API call, stop and don't store transient
    if ( is_wp_error( $response ) )
      return 'error';
	$json_string = preg_replace('/^receiveCount\((.*)\)$/', "\\1", $response);
    // Decode JSON
    $json = json_decode( $json_string );

    // Set total count
    $count = absint( $json->count );

 return absint( $count );
} }

if (!function_exists('mvp_share_count')) {
function mvp_share_count() {
	$post_id = get_the_ID(); ?>
<?php $soc_tot = get_tweets( $post_id ) + get_fb( $post_id ) + get_pinterest( $post_id ); if ($soc_tot > 999999999) {
		$soc_format = number_format($soc_tot / 1000000000, 1) . 'B';
	} else if ($soc_tot > 999999) {
		$soc_format = number_format($soc_tot / 1000000, 1) . 'M';
	} else if ($soc_tot > 999) {
        	$soc_format = number_format($soc_tot / 1000, 1) . 'K';
	} else {
		$soc_format = $soc_tot;
   	}
?>
			<?php if($soc_format==0) { ?>
			<?php } elseif($soc_format==1) { ?>
				<div class="post-soc-count left relative">
					<span class="soc-count-num"><?php echo esc_html( $soc_format ); ?></span>
					<span class="soc-count-text"><?php _e( 'Share', 'mvp-text' ); ?></span>
				</div><!--post-social-count-->
			<?php } else { ?>
				<div class="post-soc-count left relative">
					<span class="soc-count-num"><?php echo esc_html( $soc_format ); ?></span>
					<span class="soc-count-text"><?php _e( 'Shares', 'mvp-text' ); ?></span>
				</div><!--post-social-count-->
			<?php } ?>

<?php } }

/////////////////////////////////////
// Comments
/////////////////////////////////////

if ( !function_exists( 'mvp_comment' ) ) {
function mvp_comment( $comment, $args, $depth ) {
	$GLOBALS['comment'] = $comment;
	switch ( $comment->comment_type ) :
		case '' :
	?>
	<li <?php comment_class(); ?> id="li-comment-<?php comment_ID(); ?>">


		<div class="comment-wrapper" id="comment-<?php comment_ID(); ?>">
			<div class="comment-inner">

				<div class="comment-avatar">
					<?php echo get_avatar( $comment, 46 ); ?>
				</div>

				<div class="commentmeta">
					<p class="comment-meta-1">
						<?php printf( __( '%s ', 'mvp-text'), sprintf( '<cite class="fn">%s</cite>', get_comment_author_link() ) ); ?>
					</p>
					<p class="comment-meta-2">
						<?php echo get_comment_date(); ?> <?php _e( 'at', 'mvp-text'); ?> <?php echo get_comment_time(); ?>
						<?php edit_comment_link( __( 'Edit', 'mvp-text'), '(' , ')'); ?>
					</p>

				</div>

				<div class="text">

					<?php if ( $comment->comment_approved == '0' ) : ?>
						<p class="waiting_approval"><?php _e( 'Your comment is awaiting moderation.', 'mvp-text' ); ?></p>
					<?php endif; ?>

					<div class="c">
						<?php comment_text(); ?>
					</div>

				</div><!-- .text  -->
				<div class="clear"></div>
				<div class="comment-reply"><span class="reply"><?php comment_reply_link( array_merge( $args, array( 'depth' => $depth, 'max_depth' => $args['max_depth'] ) ) ); ?></span></div>
			</div><!-- comment-inner  -->
		</div><!-- comment-wrapper  -->
	<?php
			break;
		case 'pingback'  :
		case 'trackback' :
	?>
	<li class="post pingback">
		<p><?php _e( 'Pingback:', 'mvp-text' ); ?> <?php comment_author_link(); ?><?php edit_comment_link( __( 'Edit', 'mvp-text' ), ' ' ); ?></p>
	<?php
			break;
	endswitch;
}
}

/////////////////////////////////////
// Popular Posts
/////////////////////////////////////

function getCrunchifyPostViews($postID){
    $count_key = 'post_views_count';
    $count = get_post_meta($postID, $count_key, true);
    if($count==''){
        delete_post_meta($postID, $count_key);
        add_post_meta($postID, $count_key, '0');
        return "0 View";
    }
    return $count.' Views';
}

function setCrunchifyPostViews($postID) {
    $count_key = 'post_views_count';
    $count = get_post_meta($postID, $count_key, true);
    if($count==''){
        $count = 0;
        delete_post_meta($postID, $count_key);
        add_post_meta($postID, $count_key, '0');
    }else{
        $count++;
        update_post_meta($postID, $count_key, $count);
    }
}

/////////////////////////////////////
// Previous/Next Posts
/////////////////////////////////////

if ( !function_exists( 'mvp_prev_next_links' ) ) {
function mvp_prev_next_links() {

if( !is_singular('post') )
      return;

if( $next_post = get_next_post(TRUE, '') ):
echo'<div class="prev-next-item left relative">';
$nextpost = get_the_post_thumbnail( $next_post->ID, 'medium-thumb');
next_post_link( '%link',"<div class='prev-next-img left relative'>$nextpost</div><div class='prev-next-text left relative'>%title</div>", TRUE );
echo'</div>';
endif;

if( $prev_post = get_previous_post(TRUE, '') ):
echo'<div class="prev-next-item left relative">';
$prevpost = get_the_post_thumbnail( $prev_post->ID, 'medium-thumb');
previous_post_link( '%link',"<div class='prev-next-img left relative'>$prevpost</div><div class='prev-next-text left relative'>%title</div>", TRUE );
echo'</div>';
endif;
}
}

/////////////////////////////////////
// Pagination
/////////////////////////////////////

if ( !function_exists( 'pagination' ) ) {
function pagination($pages = '', $range = 4)
{
     $showitems = ($range * 2)+1;

     global $paged;
     if(empty($paged)) $paged = 1;

     if($pages == '')
     {
         global $wp_query;
         $pages = $wp_query->max_num_pages;
         if(!$pages)
         {
             $pages = 1;
         }
     }

     if(1 != $pages)
     {
         echo "<div class=\"pagination\"><span>Page ".$paged." of ".$pages."</span>";
         if($paged > 2 && $paged > $range+1 && $showitems < $pages) echo "<a href='".get_pagenum_link(1)."'>&laquo; First</a>";
         if($paged > 1 && $showitems < $pages) echo "<a href='".get_pagenum_link($paged - 1)."'>&lsaquo; Previous</a>";

         for ($i=1; $i <= $pages; $i++)
         {
             if (1 != $pages &&( !($i >= $paged+$range+1 || $i <= $paged-$range-1) || $pages <= $showitems ))
             {
                 echo ($paged == $i)? "<span class=\"current\">".$i."</span>":"<a href='".get_pagenum_link($i)."' class=\"inactive\">".$i."</a>";
             }
         }

         if ($paged < $pages && $showitems < $pages) echo "<a href=\"".get_pagenum_link($paged + 1)."\">Next &rsaquo;</a>";
         if ($paged < $pages-1 &&  $paged+$range-1 < $pages && $showitems < $pages) echo "<a href='".get_pagenum_link($pages)."'>Last &raquo;</a>";
         echo "</div>\n";
     }
}
}

/////////////////////////////////////
// Add/Remove User Contact Info
/////////////////////////////////////

if ( !function_exists( 'new_contactmethods' ) ) {
function new_contactmethods( $contactmethods ) {
    $contactmethods['facebook'] = 'Facebook'; // Add Facebook
    $contactmethods['twitter'] = 'Twitter'; // Add Twitter
    $contactmethods['pinterest'] = 'Pinterest'; // Add Pinterest
    $contactmethods['googleplus'] = 'Google Plus'; // Add Google Plus
    $contactmethods['instagram'] = 'Instagram'; // Add Instagram
    $contactmethods['linkedin'] = 'LinkedIn'; // Add LinkedIn
    unset($contactmethods['yim']); // Remove YIM
    unset($contactmethods['aim']); // Remove AIM
    unset($contactmethods['jabber']); // Remove Jabber

    return $contactmethods;
}
}
add_filter('user_contactmethods','new_contactmethods',10,1);

/////////////////////////////////////
// Footer Javascript
/////////////////////////////////////

if ( !function_exists( 'mvp_wp_footer' ) ) {
function mvp_wp_footer() {

?>

<script type="text/javascript">
jQuery(document).ready(function($) {

	// Back to Top Button
    	var duration = 500;
    	$('.back-to-top').click(function(event) {
          event.preventDefault();
          $('html, body').animate({scrollTop: 0}, duration);
          return false;
    	})

	// Main Menu Dropdown Toggle
	$(".menu-item-has-children a").click(function(e){
	  e.stopPropagation();
	  location.href = this.href;
  	});

	$(".menu-item-has-children").click(function(){
    	  $(this).addClass('toggled');
    	  if($('.menu-item-has-children').hasClass('toggled'))
    	  {
    	  $(this).children('ul').toggle();
	  $(".main-menu").getNiceScroll().resize();
	  }
	  $(this).toggleClass('tog-minus');
    	  return false;
  	});

	// Main Menu Scroll
	$(window).load(function(){
	  $(".main-menu").niceScroll({cursorcolor:"#888",cursorwidth: 7,cursorborder: 0,zindex:999999});
	  $(".head-latest-wrap").niceScroll({cursorcolor:"#ccc",cursorwidth: 7,cursorborder: 0});
	});


<?php $mvp_infinite_scroll = get_option('mvp_infinite_scroll'); if ($mvp_infinite_scroll == "true") { if (isset($mvp_infinite_scroll)) { ?>
	// Infinite Scroll
	$('.infinite-content').infinitescroll({
	  navSelector: ".nav-links",
	  nextSelector: ".nav-links a:first",
	  itemSelector: ".infinite-post",
	  loading: {
		msgText: "<?php _e( 'Loading more posts...', 'mvp-text' ); ?>",
		finishedMsg: "<?php _e( 'Sorry, no more posts', 'mvp-text' ); ?>"
	  }
	});
<?php } } ?>

});
</script>

<?php }

}
add_action( 'wp_footer', 'mvp_wp_footer' );

/////////////////////////////////////
// Site Layout
/////////////////////////////////////

if ( !function_exists( 'mvp_site_layout' ) ) {
function mvp_site_layout() {

?>

<style type="text/css">

<?php $mvp_feat_posts = get_option('mvp_feat_posts'); if ( is_page_template('page-home.php') && $mvp_feat_posts == "false" ) { ?>
<?php } else if ( is_page_template('page-home.php') || is_page_template('page-hometest.php') ) { ?>
header {
	height: auto;
	}

#main-nav-wrap {
	top: -70px;
	}
<?php } ?>

<?php if ( is_user_logged_in() && is_admin_bar_showing() && ! is_page_template('page-home.php') ) { ?>

#main-nav-wrap {
	top: 32px;
	}

<?php } ?>

<?php if ( is_single() ) { $mvp_author = get_option('mvp_author_box'); $mvp_social_box = get_option('mvp_social_box'); $post_id = get_the_ID(); $soc_tot = get_tweets( $post_id ) + get_fb( $post_id ) + get_pinterest( $post_id ); if($soc_tot==0 && $mvp_author == "false") { ?>
.post-info-left {
	padding-top: 0;
	}

.post-info-left-top {
	margin-top: 0;
	}
<?php } else if($mvp_author == "false" && $mvp_social_box == "false") { ?>
.post-info-left {
	padding-top: 0;
	}

.post-info-left-top {
	margin-top: 0;
	}
<?php } else if($soc_tot>0 && $mvp_author == "false") { ?>
.post-info-left {
	padding-top: 98px;
	}

.post-info-left-top {
	margin-top: -98px;
	}
<?php } else if($soc_tot==0 || $mvp_social_box == "false") { ?>
.post-info-left {
	padding-top: 197px;
	}

.post-info-left-top {
	margin-top: -197px;
	}
<?php } else { } } ?>

<?php if ( is_page() ) { $mvp_social_page = get_option('mvp_social_page'); $post_id = get_the_ID(); $soc_tot = get_tweets( $post_id ) + get_fb( $post_id ) + get_pinterest( $post_id ); if($soc_tot==0 || $mvp_social_page == "false") { ?>
.post-info-left {
	padding-top: 0;
	}

.post-info-left-top {
	margin-top: 0;
	}
<?php } else if($soc_tot>0) { ?>
.post-info-left {
	padding-top: 98px;
	}

.post-info-left-top {
	margin-top: -98px;
	}
<?php } else { } } ?>

<?php $more_ad = get_option('mvp_more_ad'); if ( ! $more_ad) { ?>
.single .archive-list-out,
.single .archive-list-in {
	margin-right: 0;
	}
<?php } ?>

<?php if( ! get_option('mvp_featured_ad')) { ?>
.feat-sub-out,
.feat-sub-in {
	margin-right: 0;
	}

.feat-main-sub {
	margin-left: 1.48075024679%; /* 15px / 1013px */
	width: 32.3461665021%; /* 327.66px / 1013px */
	}

.feat-main-sub:first-child {
	margin-left: 0;
	}
<?php } ?>

<?php $mvp_social_page = get_option('mvp_social_page'); if( $mvp_social_page == "false" ) { ?>
.page .post-body-out,
.page .post-body-in {
	margin-left: 0;
	overflow: visible;
	}
<?php } ?>
<?php $mvp_social_box = get_option('mvp_social_box'); $mvp_author = get_option('mvp_author_box'); if ( $mvp_social_box == "false" && $mvp_author == "false" ) { ?>
.single .post-body-out,
.single .post-body-in {
	margin-left: 0;
	overflow: visible;
	}
<?php } ?>

<?php $mvp_social_page = get_option('mvp_social_page'); $mvp_social_box = get_option('mvp_social_box'); if( is_single() && $mvp_social_box == "true" ) { ?>
@media screen and (max-width: 1002px) {
#foot-wrap {
	margin-bottom: 55px;
	}
}
<?php } else if( is_page() && $mvp_social_page == "true" && ! is_page_template('page-home.php') ) { ?>
@media screen and (max-width: 1002px) {
#foot-wrap {
	margin-bottom: 55px;
	}
}
<?php } ?>

<?php global $post; $mvp_post_temp = get_post_meta($post->ID, "mvp_post_template", true); if ( $mvp_post_temp == "full" || $mvp_post_temp == "wide-full" ) { ?>
.content-area-out,
.content-area-in {
	margin-right: 0;
	}
<?php } ?>

<?php $mvp_feat_layout = get_option('mvp_feat_layout'); if( is_home() || $mvp_feat_layout == "Featured 2" ) { ?>

@media screen and (min-width: 1601px) {
#head-main-top {
	height: 900px !important;
	}
}

@media screen and (min-width: 1003px) {
#head-main-top {
	height: 100%;
	}

.category #head-main-top {
	height: auto;
	}
}

@media screen and (max-width: 479px) {
	#main-nav-wrap {
		top: 0 !important;
		}

	header {
		height: 60px;
		}
}

<?php } else { ?>

@media screen and (max-width: 767px) {
	#main-nav-wrap {
		top: 0 !important;
		}

	header {
		height: 60px;
		}
}
<?php } ?>

<?php if( ! get_option('mvp_featured_ad')) { ?>

@media screen and (max-width: 1249px) and (min-width: 1099px) {
.feat-mobi .head-latest-text h2 {
	font-size: 1.2rem;
	}
.head-latest-wrap {
	height: 615px;
	}
.category .head-latest-wrap {
	height: 697px;
	}
.feat-mobi ul.head-latest-list {
	margin-bottom: 0;
	}
}

@media screen and (max-width: 1099px) and (min-width: 1004px) {
.feat-mobi .head-latest-text h2 {
	font-size: 1.2rem;
	}
.head-latest-wrap {
	height: 615px;
	}
.category .head-latest-wrap {
	height: 697px;
	}
.feat-mobi ul.head-latest-list {
	margin-bottom: 0;
	}
}

@media screen and (max-width: 1003px) and (min-width: 900px) {
.feat-mobi .head-latest-text h2 {
	font-size: .9rem;
	}
.head-latest-wrap {
	height: 615px;
	}
.category .head-latest-wrap {
	height: 697px;
	}
.feat-mobi ul.head-latest-list {
	margin-bottom: 0;
	}
}

@media screen and (max-width: 899px) and (min-width: 768px) {
.feat-mobi .head-latest-text h2 {
	font-size: .9rem;
	}
.head-latest-wrap {
	height: 615px;
	}
.category .head-latest-wrap {
	height: 697px;
	}
.feat-mobi ul.head-latest-list {
	margin-bottom: 0;
	}
}

<?php } ?>

</style>

<style type="text/css">
<?php $customcss = get_option('mvp_customcss'); if ($customcss) { echo wp_kses_post($customcss); } ?>
</style>

<?php }

}

add_action( 'wp_head', 'mvp_site_layout' );

/////////////////////////////////////
// Remove Pages From Search Results
/////////////////////////////////////

if ( !is_admin() ) {

function SearchFilter($query) {
if ($query->is_search) {
$query->set('post_type', 'post');
}
return $query;
}

add_filter('pre_get_posts','SearchFilter');

}

/////////////////////////////////////
// Miscellaneous
/////////////////////////////////////

// Place Wordpress Admin Bar Above Main Navigation

if ( is_user_logged_in() ) {
	if ( is_admin_bar_showing() ) {
	function mvp_admin_bar() {
		echo "
			<style type='text/css'>
			#site,.main-nav-drop,.fly-but-wrap,#fly-wrap {top: 32px !important;}
			</style>
		";
	}
	add_action( 'wp_head', 'mvp_admin_bar' );
	}
}

// Set Content Width
if ( ! isset( $content_width ) ) $content_width = 620;

// Add RSS links to <head> section - moved to theme setup

add_action('init', 'do_output_buffer');
function do_output_buffer() {
        ob_start();
}

// Prevents double posts on second page

add_filter('redirect_canonical','pif_disable_redirect_canonical');

function pif_disable_redirect_canonical($redirect_url) {
    if (is_singular()) $redirect_url = false;
return $redirect_url;
}

// Prevents duplicate page titles

add_filter('wpseo_title', 'fix_archive_titles');
function fix_archive_titles($title) {
    if (is_category()) {
        $title = single_cat_title('', false) . ' Gaming News & Reviews | Gaming Debugged';
    }
    if (is_tag()) {
        $title = single_tag_title('', false) . ' Games & Articles | Gaming Debugged';
    }
    if (is_author()) {
        $title = 'Articles by ' . get_the_author() . ' | Gaming Debugged';
    }
    if (is_date()) {
        if (is_year()) {
            $title = get_the_date('Y') . ' Gaming Archives | Gaming Debugged';
        } elseif (is_month()) {
            $title = get_the_date('F Y') . ' Gaming News | Gaming Debugged';
        }
    }
    if (is_paged()) {
        $title .= ' - Page ' . get_query_var('paged');
    }
    return $title;
}

/////////////////////////////////////
// WooCommerce
/////////////////////////////////////

add_theme_support( 'woocommerce' );

?>
