import { useEffect, useState } from 'react'
import './App.css'

const apiUrl = import.meta.env.VITE_API_URL?.replace(/\/+$/, '')
const priceFormat = new Intl.NumberFormat('en-LK', {
  style: 'currency',
  currency: 'LKR',
  maximumFractionDigits: 0,
})

function App() {
  const [items, setItems] = useState([])
  const [loading, setLoading] = useState(Boolean(apiUrl))
  const [error, setError] = useState(apiUrl ? '' : 'The API address is not configured yet.')
  const [reload, setReload] = useState(0)

  useEffect(() => {
    if (!apiUrl) {
      return
    }

    const controller = new AbortController()

    async function loadMenu() {
      try {
        const response = await fetch(`${apiUrl}/api/menu`, {
          signal: controller.signal,
        })
        if (!response.ok) {
          throw new Error(`The menu request failed (${response.status}).`)
        }
        setItems(await response.json())
      } catch (requestError) {
        if (requestError.name !== 'AbortError') {
          setError(requestError.message || 'Could not load the menu.')
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }

    loadMenu()
    return () => controller.abort()
  }, [reload])

  function refreshMenu() {
    setLoading(true)
    setError('')
    setReload((value) => value + 1)
  }

  return (
    <main className="page-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="CampusEats home">
          <span className="brand-mark" aria-hidden="true">C</span>
          <span>Campus<span className="brand-accent">Eats</span></span>
        </a>
        <span className="campus-label">YOUR CAMPUS, YOUR TABLE</span>
      </header>

      <section className="hero">
        <div className="hero-copy">
          <p className="eyebrow"><span className="eyebrow-dot" /> FRESH FROM CAMPUS</p>
          <h1>Good food.<br /><span>Good days.</span></h1>
          <p className="hero-description">
            Your campus favourites, made fresh and ready when you are.
          </p>
          <a className="menu-link" href="#menu">Explore today&apos;s menu <span aria-hidden="true">↓</span></a>
        </div>
        <div className="hero-art" aria-hidden="true">
          <div className="plate">
            <span className="plate-leaf leaf-one" />
            <span className="plate-leaf leaf-two" />
            <span className="plate-leaf leaf-three" />
            <span className="plate-tomato" />
            <span className="plate-rice" />
          </div>
          <span className="art-spark spark-one">✳</span>
          <span className="art-spark spark-two">✳</span>
          <span className="art-note">made with<br />a little love</span>
        </div>
        <div className="hero-stamp">GOOD<br />MOOD<br />FOOD</div>
      </section>

      <section className="menu-section" id="menu">
        <div className="section-heading">
          <div>
            <p className="eyebrow">A LITTLE SOMETHING FOR EVERYONE</p>
            <h2>Today&apos;s menu<span>.</span></h2>
          </div>
          <button className="refresh-button" onClick={refreshMenu} disabled={loading}>
            <span aria-hidden="true">↻</span> Refresh menu
          </button>
        </div>

        {loading && <p className="notice" role="status">Getting the good stuff ready…</p>}
        {!loading && error && (
          <div className="notice notice-error" role="alert">
            <span>{error}</span>
            {apiUrl && <button onClick={refreshMenu}>Try again</button>}
          </div>
        )}
        {!loading && !error && items.length === 0 && (
          <p className="notice">The menu is empty right now. Check back soon!</p>
        )}
        {!loading && !error && items.length > 0 && (
          <div className="menu-grid">
            {items.map((item, index) => (
              <article className="menu-card" key={item.id}>
                <div className={`dish-art dish-art-${index % 3}`} aria-hidden="true">
                  <span className="dish-plate"><span /></span>
                </div>
                <div className="dish-details">
                  <div className="dish-meta">
                    <span className="dish-category">{item.category}</span>
                    {item.available && <span className="available"><span /> Available</span>}
                  </div>
                  <div className="dish-title-row">
                    <h3>{item.name}</h3>
                    <span className="dish-price">{priceFormat.format(item.price)}</span>
                  </div>
                </div>
              </article>
            ))}
          </div>
        )}
      </section>

      <footer className="footer">
        <span>Made for campus life <span className="footer-heart">♥</span></span>
        <span>CampusEats <span className="footer-year">· Fresh every day</span></span>
      </footer>
    </main>
  )
}

export default App
